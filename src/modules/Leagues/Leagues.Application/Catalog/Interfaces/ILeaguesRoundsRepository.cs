using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Interfaces;

public interface ILeaguesRoundsRepository
{
    Task<IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>>> GetRoundsFromAllActiveVersionsAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LeagueRoundDto>> GetAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);
}
