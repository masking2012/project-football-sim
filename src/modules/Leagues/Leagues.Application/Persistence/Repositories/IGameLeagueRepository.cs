using ProjectFootballSim.Leagues.Application.Persistence.Models;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.Persistence.Repositories;

public interface IGameLeagueRepository
{
    Task<IReadOnlyList<int>> GetLeagueVersionIdsAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<int>> GetTeamIdsAsync(
        int leagueVersionId,
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

    Task<IReadOnlyList<GameLeagueMatchByDate>> GetMatchesByDateAsync(
        Guid userId,
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken);

}
