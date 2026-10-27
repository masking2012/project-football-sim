using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.Persistence.Repositories;

public interface IGameLeagueWriteRepository
{
    Task AddAsync(GameLeague gameLeague, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
