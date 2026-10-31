using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.Calendar.Application.Features.CompleteGameSeason;
using ProjectFootballSim.Calendar.Contracts.Features.CompleteGameSeason;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Contracts.Features.GetGameSeasons;
using System.Security.Claims;

namespace ProjectFootballSim.Api.Endpoints.Seasons;

internal static class GameSeasonsEndpoints
{
    public static void MapGameSeasonsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/games/{gameId}/seasons/{seasonId}/complete", async (
            [FromRoute] Guid gameId,
            [FromRoute] Guid seasonId,
            ICompleteGameSeasonCommandHandler completeGameSeasonCommandHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var completeGameSeasonCommand = new CompleteGameSeasonCommand(
                UserId: userId,
                GameId: gameId,
                SeasonId: seasonId);
            await completeGameSeasonCommandHandler
            .HandleAsync(completeGameSeasonCommand, cancellationToken)
            .ConfigureAwait(false);

            return Results.NoContent();
        }).RequireAuthorization();

        app.MapPost("/api/games/{gameId}/seasons", async (
            [FromRoute] Guid gameId,
            ICreateGameSeasonCommandHandler createGameSeasonCommandHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var createGameSeasonCommandResult = await createGameSeasonCommandHandler
                .HandleAsync(new CreateGameSeasonCommand(UserId: userId, GameId: gameId), cancellationToken)
                .ConfigureAwait(false);

            return Results.Ok(new CreateGameSeasonResponse(createGameSeasonCommandResult.Id));
        }).RequireAuthorization();

        app.MapGet("/api/games/{gameId}/seasons", async (
            [FromRoute] Guid gameId,
            IGetGameSeasonsQueryHandler queryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var query = new GetGameSeasonsQuery(GameId: gameId, UserId: userId);
            var gameSeasons = await queryHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);

            //TODO: remove GameSeasonItemResponse once we have a proper mapping layer in place
            return Results.Ok(
                gameSeasons
                    .Select(s => new GameSeasonItemResponse(Id: s.Id, StartDate: s.StartDate, EndDate: s.EndDate, IsCurrent: s.IsCurrent)));
        }).RequireAuthorization();
    }
}
