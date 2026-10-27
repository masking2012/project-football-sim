using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

public interface ILeaguesRoundsRepository
{
    Task<IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>>> GetActiveAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LeagueRoundDto>> GetForVersionAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);
}
