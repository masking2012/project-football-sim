using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.GameState.Services;

public interface IGameLeagueRepository
{
    Task<IReadOnlyList<int>> GetLeagueVersionIdsAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, int>> GetLeagueVersionIdsByGameLeagueAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken);

    Task<GameLeague?> GetAsync(
        Guid userId,
        Guid gameId,
        Guid seasonId,
        int leagueVersionId,
        CancellationToken cancellationToken);

    Task<GameLeague?> GetAsync(
        Guid userId,
        Guid gameLeagueId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<GameLeagueMatch>> GetMatchesForDateAsync(
        Guid userId,
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken);

}
