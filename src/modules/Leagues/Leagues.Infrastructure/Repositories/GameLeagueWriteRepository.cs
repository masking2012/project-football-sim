using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class GameLeagueWriteRepository(LeaguesDbContext dbContext) : IGameLeagueWriteRepository
{
    public Task<bool> ExistsAsync(Guid gameId, Guid seasonId, int leagueVersionId, CancellationToken cancellationToken) =>
        dbContext.GameLeagues.AnyAsync(
            gl => gl.GameId == gameId && gl.SeasonId == seasonId && gl.LeagueVersionId == leagueVersionId,
            cancellationToken);

    public Task AddAsync(GameLeague gameLeague, CancellationToken cancellationToken)
    {
        dbContext.GameLeagues.Add(gameLeague);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
