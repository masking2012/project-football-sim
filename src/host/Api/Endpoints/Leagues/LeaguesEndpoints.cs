using ProjectFootballSim.Leagues.Application.Features.GetLeagues;

namespace ProjectFootballSim.Api.Endpoints.Leagues;

internal static class LeaguesEndpoints
{
    public static void MapLeaguesEndpoints(this WebApplication app)
    {
        app.MapGet("/api/leagues", async (
            GetLeaguesQueryHandler queryHandler,
            CancellationToken cancellationToken) =>
        {
            var leagueDtos = await queryHandler.HandleAsync(cancellationToken).ConfigureAwait(false);

            return Results.Ok(leagueDtos.Values.Select(l => new LeagueItemResponse(
                Id: l.Id,
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
    }
}
