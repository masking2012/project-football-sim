using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.Leagues.Application.Features.GetLeagueById;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueFixtures;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueStandings;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;
using System.Security.Claims;

namespace ProjectFootballSim.Api.Endpoints.Leagues;

internal static class GameLeaguesEndpoints
{
    public static void MapGameLeaguesEndpoints(this WebApplication app)
    {
        app.MapGet("/api/games/{gameId}/seasons/{seasonId}/leagues/{leagueId}/standings", async (
            [FromRoute] int leagueId,
            [FromRoute] Guid gameId,
            [FromRoute] Guid seasonId,
            GetGameLeagueStandingsQueryHandler getGameLeagueStandingsQueryHandler,
            GetTeamsByCountryQueryHandler teamsByCountryQueryHandler,
            GetLeagueByIdQueryHandler getLeagueByIdQueryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var leagueDto = await getLeagueByIdQueryHandler.HandleAsync(leagueId, cancellationToken).ConfigureAwait(false);
            if (leagueDto is null)
                throw new InvalidOperationException($"League with ID {leagueId} not found.");

            var teamDtos = await teamsByCountryQueryHandler.HandleAsync(leagueDto.CountryId, cancellationToken).ConfigureAwait(false);

            var query = new GetGameLeagueStandingsQuery(
                UserId: userId,
                GameId: gameId,
                SeasonId: seasonId,
                LeagueId: leagueId);
            var gameLeagueStandings = await getGameLeagueStandingsQueryHandler
                .HandleAsync(query, cancellationToken).ConfigureAwait(false);

            return Results.Ok(
                gameLeagueStandings
                    .Select(s => new TeamStandingItemResponse(
                        Position: s.Position,
                        TeamId: s.TeamId,
                        Name: teamDtos.Single(t => t.Id == s.TeamId).Name, //TODO: Optimize this by creating a dictionary of teamDtos by Id to avoid multiple enumerations
                        Wins: s.Wins,
                        Draws: s.Draws,
                        Losses: s.Losses,
                        GoalsFor: s.GoalsFor,
                        GoalsAgainst: s.GoalsAgainst,
                        Points: s.Points)));
        }).RequireAuthorization();

        app.MapGet("/api/games/{gameId}/seasons/{seasonId}/leagues/{leagueId}/fixtures", async (
            [FromRoute] int leagueId,
            [FromRoute] Guid gameId,
            [FromRoute] Guid seasonId,
            GetGameLeagueFixturesQueryHandler getGameLeagueFixturesQueryHandler,
            GetTeamsByCountryQueryHandler teamsByCountryQueryHandler,
            GetLeagueByIdQueryHandler getLeagueByIdQueryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var leagueDto = await getLeagueByIdQueryHandler.HandleAsync(leagueId, cancellationToken).ConfigureAwait(false);
            if (leagueDto is null)
                throw new InvalidOperationException($"League with ID {leagueId} not found.");

            var teamDtos = await teamsByCountryQueryHandler.HandleAsync(leagueDto.CountryId, cancellationToken).ConfigureAwait(false);

            var query = new GetGameLeagueFixturesQuery(
                UserId: userId,
                GameId: gameId,
                SeasonId: seasonId,
                LeagueId: leagueId);
            var matchesDtos = await getGameLeagueFixturesQueryHandler
                .HandleAsync(query, cancellationToken).ConfigureAwait(false);

            return Results.Ok(
                matchesDtos
                    .Select(s => new MatchItemResponse(
                        Id: s.Id,
                        Date: s.Date,
                        HomeTeamId: s.HomeTeamId,
                        AwayTeamId: s.AwayTeamId,
                        HomeTeamScore: s.HomeTeamScore,
                        AwayTeamScore: s.AwayTeamScore,
                        Round: s.Round,
                        HomeTeamName: teamDtos.Single(t => t.Id == s.HomeTeamId).Name, //TODO: Optimize this by creating a dictionary of teamDtos by Id to avoid multiple enumerations
                        AwayTeamName: teamDtos.Single(t => t.Id == s.AwayTeamId).Name)));
        }).RequireAuthorization();
    }
}
