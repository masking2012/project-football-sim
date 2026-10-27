using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

public interface ICachedLeaguesRetriever
{
    ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllActiveLeaguesAsync(
        CancellationToken cancellationToken);

    ValueTask<LeagueDto?> GetLeagueAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<LeagueDto>> GetLeaguesAsync(
        IReadOnlyCollection<int> leagueVersionIds,
        CancellationToken cancellationToken);
}
