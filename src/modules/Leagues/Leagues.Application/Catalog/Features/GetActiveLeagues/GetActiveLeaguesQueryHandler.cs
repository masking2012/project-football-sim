using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;

namespace ProjectFootballSim.Leagues.Application.Catalog.Features.GetActiveLeagues;

public sealed class GetActiveLeaguesQueryHandler(ILeaguesCatalog leagueCatalog)
{
    public ValueTask<IReadOnlyDictionary<int, LeagueDto>> HandleAsync(
        CancellationToken cancellationToken) =>
        leagueCatalog.GetAllActiveAsync(cancellationToken);
}
