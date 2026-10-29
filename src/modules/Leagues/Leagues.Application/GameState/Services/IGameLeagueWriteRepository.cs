using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.GameState.Services;

public interface IGameLeagueWriteRepository
{
    Task<bool> ExistsAsync(Guid gameId, Guid seasonId, int leagueVersionId, CancellationToken cancellationToken);

    Task AddAsync(GameLeague gameLeague, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
