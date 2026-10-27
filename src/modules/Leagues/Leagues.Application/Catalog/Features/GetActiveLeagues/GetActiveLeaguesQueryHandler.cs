using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;

namespace ProjectFootballSim.Leagues.Application.Catalog.Features.GetActiveLeagues;

public sealed class GetActiveLeaguesQueryHandler(ICachedLeaguesRetriever leaguesRetriever)
{
    public ValueTask<IReadOnlyDictionary<int, LeagueDto>> HandleAsync(
        CancellationToken cancellationToken) =>
        leaguesRetriever.GetAllActiveLeaguesAsync(cancellationToken);
}
