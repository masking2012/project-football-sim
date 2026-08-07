using ProjectFootballSim.Leagues.Application.Common.Models;

namespace ProjectFootballSim.Leagues.Application.Common.Services;

public interface ILeaguesRoundsCatalog
{
    ValueTask<IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>>> GetAllAsync(
        CancellationToken cancellationToken);
}
