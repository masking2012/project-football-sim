using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

public interface ILeaguesRepository
{
    Task<IReadOnlyDictionary<int, LeagueDto>> GetActiveAsync(CancellationToken cancellationToken);

    Task<LeagueDto?> GetByLeagueVersionIdAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);
}
