using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.Leagues.Application.Catalog.Features.GetLeagueByVersionId;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagues;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueFixtures;
using System.Security.Claims;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByCountry;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueStandings;

namespace ProjectFootballSim.Api.Endpoints.Leagues;

internal static class GameLeaguesEndpoints
{
    public static void MapGameLeaguesEndpoints(this WebApplication app)
    {
        app.MapGet("/api/games/{gameId}/leagues", async (
            [FromRoute] Guid gameId,
            GetGameLeaguesQueryHandler getGameLeaguesQueryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var query = new GetGameLeaguesQuery(
                UserId: userId,
                GameId: gameId);
            IReadOnlyList<LeagueDto> leagues = await getGameLeaguesQueryHandler
                .HandleAsync(query, cancellationToken).ConfigureAwait(false);

            return Results.Ok(leagues.Select(l => new LeagueItemResponse(
                Id: l.LeagueVersionId,
                Name: l.Name,
                Order: l.Order,
                CountryId: l.CountryId,
                TeamsCount: l.TeamsCount,
                PromotionPositions: l.PromotionPositions,
                PromotionPlayOffPositions: l.PromotionPlayOffPositions,
                RelegationPositions: l.RelegationPositions,
                RelegationPlayOffPositions: l.RelegationPlayOffPositions,
                UefaChampionsLeaguePositions: l.UefaChampionsLeaguePositions,
                UefaEuropaLeaguePositions: l.UefaEuropaLeaguePositions,
                UefaConferenceLeaguePositions: l.UefaConferenceLeaguePositions)));
        }).RequireAuthorization();

        app.MapGet("/api/games/{gameId}/seasons/{seasonId}/leagues/{leagueVersionId}/standings", async (
            [FromRoute] Guid gameId,
            [FromRoute] Guid seasonId,
            [FromRoute] int leagueVersionId,
            IGetGameLeagueStandingsQueryHandler getGameLeagueStandingsQueryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var query = new GetGameLeagueStandingsQuery(
                UserId: userId,
                GameId: gameId,
                SeasonId: seasonId,
                LeagueVersionId: leagueVersionId);
            IReadOnlyList<LeagueStandingsTeamDto> gameLeagueStandings = await getGameLeagueStandingsQueryHandler
                .HandleAsync(query, cancellationToken).ConfigureAwait(false);

            return Results.Ok(gameLeagueStandings);
        }).RequireAuthorization();
        
        app.MapGet("/api/games/{gameId}/seasons/{seasonId}/leagues/{leagueVersionId}/fixtures", async (
            [FromRoute] Guid gameId,
            [FromRoute] Guid seasonId,
            [FromRoute] int leagueVersionId,
            GetGameLeagueFixturesQueryHandler getGameLeagueFixturesQueryHandler,
            IGetTeamsByCountryQueryHandler teamsByCountryQueryHandler,
            GetLeagueByVersionIdQueryHandler getLeagueByVersionIdQueryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var leagueDto = await getLeagueByVersionIdQueryHandler.HandleAsync(leagueVersionId, cancellationToken).ConfigureAwait(false);
            if (leagueDto is null)
                throw new InvalidOperationException($"League with version ID {leagueVersionId} not found.");

            var teamDtos = await teamsByCountryQueryHandler.HandleAsync(leagueDto.CountryId, cancellationToken).ConfigureAwait(false);

            var query = new GetGameLeagueFixturesQuery(
                UserId: userId,
                GameId: gameId,
                SeasonId: seasonId,
                LeagueVersionId: leagueVersionId);
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
                        HomeTeamName: teamDtos[s.HomeTeamId].Name, //TODO: Optimize this by creating a dictionary of teamDtos by Id to avoid multiple enumerations
                        AwayTeamName: teamDtos[s.AwayTeamId].Name)));
        }).RequireAuthorization();
    }
}
