using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

public interface ILeaguesCatalog
{
    ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllActiveAsync(
        CancellationToken cancellationToken);

    ValueTask<LeagueDto?> GetByLeagueVersionIdAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);
}
