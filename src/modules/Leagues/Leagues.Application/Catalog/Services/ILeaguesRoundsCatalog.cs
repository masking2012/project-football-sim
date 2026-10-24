using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

public interface ILeaguesRoundsCatalog
{
    ValueTask<IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>>> GetAllAsync(
        CancellationToken cancellationToken);
}
