using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
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
            SaveGameCommandHandler commandHandler,
            [FromBody] SaveGameRequest request,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var command = new SaveGameCommand(
                UserId: userId,
                GameId: request.GameId,
                SlotId: request.SlotId,
                Name: request.Name
            );
            await commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            return Results.StatusCode(201);
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
