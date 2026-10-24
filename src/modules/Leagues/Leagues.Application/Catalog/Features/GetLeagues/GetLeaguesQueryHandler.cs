using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;

namespace ProjectFootballSim.Leagues.Application.Catalog.Features.GetLeagues;

public sealed class GetLeaguesQueryHandler(ILeaguesCatalog leagueCatalog)
{
    public ValueTask<IReadOnlyDictionary<int, LeagueDto>> HandleAsync(
        CancellationToken cancellationToken) =>
        leagueCatalog.GetAllAsync(cancellationToken);
}
