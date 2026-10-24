using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

public interface ILeaguesCatalog
{
    ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllAsync(
        CancellationToken cancellationToken);

    ValueTask<LeagueDto?> GetByIdAsync(
        int leagueId,
        CancellationToken cancellationToken);
}
