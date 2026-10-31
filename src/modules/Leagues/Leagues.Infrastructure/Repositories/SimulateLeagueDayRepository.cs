using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Features.SimulateLeagueGameDay;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class SimulateLeagueDayRepository(LeaguesDbContext dbContext) : ISimulateLeagueDayRepository
{
    public async Task<IReadOnlyList<GameLeague>> GetLeaguesWithMatchesForDateAsync(
        Guid userId,
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        var gameLeagues = await dbContext.GameLeagues
             .Include(gl => gl.GameLeagueMatches
                 .Where(m => m.Date < endDate && m.Date >= startDate))
             .Include(gl => gl.GameLeagueTeams)
             .Where(gl => gl.UserId == userId
                 && gl.GameId == gameId
                 && gl.GameLeagueMatches.Any(m => m.Date < endDate && m.Date >= startDate))
             .ToListAsync(cancellationToken)
             .ConfigureAwait(false);
        return gameLeagues;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
