using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.Calendar.Application.Features.CompleteGameSeason;
using ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Application.Features.GetGameSeasons;
using ProjectFootballSim.Leagues.Application.Features.GetLeagues;
using ProjectFootballSim.Leagues.Application.GameFeatures.CreateGameLeague;
using System.Security.Claims;

namespace ProjectFootballSim.Api.Endpoints.Seasons;

internal static class GameSeasonsEndpoints
{
    public static void MapGameSeasonsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/games/{gameId}/seasons/{seasonId}/complete", async (
            [FromRoute] Guid gameId,
            [FromRoute] Guid seasonId,
            CompleteGameSeasonCommandHandler completeGameSeasonCommandHandler,
            GetLeaguesQueryHandler getLeaguesQueryHandler,
            CreateGameLeagueCommandHandler createGameLeagueCommandHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var completeGameSeasonCommand = new CompleteGameSeasonCommand(
                UserId: userId,
                GameId: gameId,
                SeasonId: seasonId);
            var completeGameSeasonResult = await completeGameSeasonCommandHandler.HandleAsync(completeGameSeasonCommand, cancellationToken).ConfigureAwait(false);

            var leaguesDtos = await getLeaguesQueryHandler.HandleAsync(cancellationToken).ConfigureAwait(false);
            foreach (var leagueDto in leaguesDtos.Values)
            {
                var createGameLeagueCommand = new CreateGameLeagueCommand(
                    LeagueId: leagueDto.Id,
                    GameId: gameId,
                    UserId: userId,
                    SeasonId: completeGameSeasonResult.Id,
                    SeasonStartDate: completeGameSeasonResult.StartDate,
                    PreviousSeasonId: seasonId);
                await createGameLeagueCommandHandler.HandleAsync(createGameLeagueCommand, cancellationToken).ConfigureAwait(false);
            }

            return Results.Created($"/api/games/{gameId}/seasons/{completeGameSeasonResult.Id}", new CreateGameSeasonResponse(completeGameSeasonResult.Id));
        }).RequireAuthorization();

        app.MapGet("/api/games/{gameId}/seasons", async (
            [FromRoute] Guid gameId,
            GetGameSeasonsQueryHandler queryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var query = new GetGameSeasonsQuery(GameId: gameId, UserId: userId);
            var gameSeasons = await queryHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);

            return Results.Ok(
                gameSeasons
                    .Select(s => new GameSeasonItemResponse(Id: s.Id, StartDate: s.StartDate, EndDate: s.EndDate, IsCurrent: s.IsCurrent)));
        }).RequireAuthorization();
    }
}
