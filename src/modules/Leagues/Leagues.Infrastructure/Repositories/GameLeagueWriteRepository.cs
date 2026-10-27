using ProjectFootballSim.Leagues.Application.Persistence.Repositories;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class GameLeagueWriteRepository(LeaguesDbContext dbContext) : IGameLeagueWriteRepository
{

    public Task AddAsync(GameLeague gameLeague, CancellationToken cancellationToken)
    {
        dbContext.GameLeagues.Add(gameLeague);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
