using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.GameState.Interfaces;

public interface IGameLeaguesRetriever
{
    Task<IReadOnlyList<LeagueDto>> GetLeaguesAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken);
}
