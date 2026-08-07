using ProjectFootballSim.Leagues.Application.Common.Models;
using ProjectFootballSim.Leagues.Application.Common.Services;

namespace ProjectFootballSim.Leagues.Application.Features.GetLeagues;

public sealed class GetLeaguesQueryHandler(ILeaguesCatalog leagueCatalog)
{
    public ValueTask<IReadOnlyDictionary<int, LeagueDto>> HandleAsync(
        CancellationToken cancellationToken) =>
        leagueCatalog.GetAllAsync(cancellationToken);
}
