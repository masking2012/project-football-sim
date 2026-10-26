using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.GameState.Services;

public interface IGameLeagueCatalog
{
    Task<IReadOnlyList<LeagueDto>> GetByGameAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken);
}
