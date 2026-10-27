using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Interfaces;

public interface ILeaguesRetriever
{
    Task<IReadOnlyDictionary<int, LeagueDto>> GetAllActiveAsync(CancellationToken cancellationToken);

    Task<LeagueDto?> GetAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);
}
