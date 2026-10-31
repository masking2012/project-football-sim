using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.GameState.Interfaces;

public interface IGameLeaguesService
{
    Task<IReadOnlyList<LeagueDto>> GetGameLeaguesAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken);
}
