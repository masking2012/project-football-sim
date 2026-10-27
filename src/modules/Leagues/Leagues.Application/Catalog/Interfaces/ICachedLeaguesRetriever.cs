using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Interfaces;

public interface ICachedLeaguesRetriever
{
    ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllActiveLeaguesAsync(
        CancellationToken cancellationToken);

    ValueTask<LeagueDto?> GetLeagueAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);

    ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetLeaguesAsync(
        IReadOnlyCollection<int> leagueVersionIds,
        CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<LeagueRoundDto>> GetLeagueRoundsAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);
}
