using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.Leagues.Application.CreateGameLeague;
using ProjectFootballSim.Leagues.Application.GetLeagues;
using ProjectFootballSim.Seasons.Application.Features.CreatePlayerSeason;
using ProjectFootballSim.Seasons.Application.Features.GetPlayerSeasons;
using System.Security.Claims;

namespace ProjectFootballSim.Api.Endpoints.Seasons;

internal static class SeasonsEndpoints
{
    public static void MapSeasonsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/seasons", async (
            [FromBody] CreateSeasonRequest request,
            CreatePlayerSeasonCommandHandler createPlayerSeasonCommandHandler,
            GetLeaguesQueryHandler getLeaguesQueryHandler,
            CreateGameLeagueCommandHandler createGameLeagueCommandHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var command = new CreatePlayerSeasonCommand(
                GameId: request.GameId,
                UserId: userId,
                CurrentGameDate: request.CurrentGameDate);
            var result = await createPlayerSeasonCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            var leaguesDtos = await getLeaguesQueryHandler.HandleAsync(cancellationToken).ConfigureAwait(false);
            foreach (var leagueDto in leaguesDtos.Values)
            {
                var createGameLeagueCommand = new CreateGameLeagueCommand(
                    LeagueId: leagueDto.Id,
                    GameId: request.GameId,
                    UserId: userId);
                await createGameLeagueCommandHandler.HandleAsync(createGameLeagueCommand, cancellationToken).ConfigureAwait(false);
            }

            return Results.Created($"/api/seasons/{result.Id}", new CreateSeasonResponse(result.Id));
        }).RequireAuthorization();

        app.MapGet("/api/seasons", async (
            [FromQuery] Guid gameId,
            GetPlayerSeasonsQueryHandler queryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var query = new GetPlayerSeasonsQuery(GameId: gameId, UserId: userId);
            var playerSeasons = await queryHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);

            return Results.Ok(
                playerSeasons
                    .Select(s => new PlayerSeasonItemResponse(Id: s.Id, StartDate: s.StartDate, EndDate: s.EndDate, IsCurrent: s.IsCurrent)));
        }).RequireAuthorization();
    }
}
