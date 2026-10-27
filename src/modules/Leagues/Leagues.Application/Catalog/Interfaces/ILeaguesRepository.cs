using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Interfaces;

public interface ILeaguesRepository
{
    Task<IReadOnlyDictionary<int, LeagueDto>> GetAllActiveAsync(CancellationToken cancellationToken);

    Task<LeagueDto?> GetAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);
}
