using ProjectFootballSim.Leagues.Application.Common.Models;

namespace ProjectFootballSim.Leagues.Application.Common.Services;

public interface ILeaguesCatalog
{
    ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllAsync(
        CancellationToken cancellationToken);

    ValueTask<LeagueDto?> GetByIdAsync(
        int leagueId,
        CancellationToken cancellationToken);
}
