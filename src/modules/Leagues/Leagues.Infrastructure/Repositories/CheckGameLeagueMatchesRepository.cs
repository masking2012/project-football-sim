using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Features.CheckGameLeagueMatchesByDate;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class CheckGameLeagueMatchesRepository(LeaguesDbContext dbContext) : ICheckGameLeagueMatchesRepository
{
    public async Task<bool> MatchesExist(Guid userId, Guid gameId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var exist = await dbContext.GameLeagues
            .AsNoTracking()
            .Where(gl => gl.UserId == userId && gl.GameId == gameId)
            .Join(
                dbContext.GameLeagueMatches.AsNoTracking()
                    .Where(m => m.Date < endDate && m.Date >= startDate),
                league => league.Id,
                match => match.GameLeagueId,
                (league, match) => new { match, league.LeagueVersionId })
            .AnyAsync(cancellationToken)
            .ConfigureAwait(false);
        return exist;
    }
}
