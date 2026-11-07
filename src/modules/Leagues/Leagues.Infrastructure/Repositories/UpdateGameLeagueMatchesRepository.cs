using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Features.UpdateGameLeagueMatches;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class UpdateGameLeagueMatchesRepository(LeaguesDbContext dbContext)
    : IUpdateGameLeagueMatchesRepository
{
    public async Task<IReadOnlyList<GameLeagueMatch>> GetMatchesAsync(
        Guid userId,
        Guid gameId,
        IReadOnlyList<Guid> matchIds,
        CancellationToken cancellationToken)
    {
        return await dbContext.GameLeagueMatches
            .Where(match => matchIds.Contains(match.Id))
            .Where(match => dbContext.GameLeagues.Any(gameLeague =>
                gameLeague.Id == match.GameLeagueId && gameLeague.UserId == userId && gameLeague.GameId == gameId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
