using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.Leagues.Application.GetGameLeagueStandings;
using ProjectFootballSim.Leagues.Application.GetLeagueById;
using ProjectFootballSim.Leagues.Application.GetLeaguesByCountries;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;
using System.Security.Claims;

namespace ProjectFootballSim.Api.Endpoints.Leagues;

internal static class GameLeaguesEndpoints
{
    public static void MapGameLeaguesEndpoints(this WebApplication app)
    {
        app.MapGet("/api/game/{gameId}/leagues/{leagueId}/standings", async (
            [FromRoute] int leagueId,
            [FromRoute] Guid gameId,
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

            var query = new GetGameLeagueStandingsQuery(GameId: gameId, UserId: userId, LeagueId: leagueId);
            var gameLeagueStandings = await getGameLeagueStandingsQueryHandler
                .HandleAsync(query, cancellationToken).ConfigureAwait(false);

            return Results.Ok(
                gameLeagueStandings
                    .Select(s => new TeamStandingItemResponse(
                        TeamId: s.TeamId,
                        Name: teamDtos.Single(t => t.Id == s.TeamId).Name, //TODO: Optimize this by creating a dictionary of teamDtos by Id to avoid multiple enumerations
                        Wins: s.Wins,
                        Draws: s.Draws,
                        Losses: s.Losses,
                        GoalsFor: s.GoalsFor,
                        GoalsAgainst: s.GoalsAgainst,
                        Points: s.Points)));
        }).RequireAuthorization();
    }
}
