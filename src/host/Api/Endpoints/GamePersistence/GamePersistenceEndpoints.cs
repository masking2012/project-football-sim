using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.GamePersistence.Application.Features.CreateGame;
using ProjectFootballSim.GamePersistence.Application.Features.LoadGames;
using ProjectFootballSim.GamePersistence.Application.Features.SaveGame;
using System.Security.Claims;

namespace ProjectFootballSim.Api.Endpoints.GamePersistence;

internal static class GamePersistenceEndpoints
{
    public static void MapGamePersistenceEndpoints(this WebApplication app)
    {
        app.MapPost("/api/games", async (
            ClaimsPrincipal user,
            GameInitializationService service,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var response = await service.InitAsync(userId, cancellationToken).ConfigureAwait(false);

            var createdUri = new Uri($"/api/games/{response.GameId}", UriKind.Relative);
            return Results.Created(createdUri, response);
        }).RequireAuthorization();

        app.MapPost("/api/games/{gameId}/save", async (
            [FromRoute] Guid gameId,
            [FromBody] SaveGameRequest request,
            ClaimsPrincipal user,
            SaveGameCommandHandler commandHandler,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var command = new SaveGameCommand(
                UserId: userId,
                GameId: gameId,
                SlotId: request.SlotId,
                Name: request.Name
            );
            await commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return Results.Ok();
        }).RequireAuthorization();

        app.MapGet("/api/games", async (
            ClaimsPrincipal user,
            LoadGamesQueryHandler query,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var gameSaves = await query.HandleAsync(userId, cancellationToken).ConfigureAwait(false);
            return Results.Ok(gameSaves.Select(gs => new GameSaveItemResponse(
                GameId: gs.GameId,
                SlotId: gs.SlotId,
                Name: gs.Name,
                CreatedAtUtc: gs.CreatedAtUtc
            )));
        }).RequireAuthorization();
    }
}
