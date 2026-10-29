using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Contracts.CatalogFeatures.GetActiveLeagues;

namespace ProjectFootballSim.Leagues.Application.Catalog.Features.GetActiveLeagues;

internal sealed class GetActiveLeaguesQueryHandler(ICachedLeaguesRetriever leaguesRetriever) : IGetActiveLeaguesQueryHandler
{
    public async Task<IReadOnlyDictionary<int, ActiveLeagueDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var activeLeagues = await leaguesRetriever.GetAllActiveLeaguesAsync(cancellationToken).ConfigureAwait(false);
        return activeLeagues.ToDictionary(
            league => league.Key,
            league => new ActiveLeagueDto(LeagueVersionId: league.Value.LeagueVersionId, Name: league.Value.Name));
    }
}
