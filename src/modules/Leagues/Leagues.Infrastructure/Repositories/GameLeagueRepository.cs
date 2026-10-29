using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Interfaces;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class GameLeagueRepository(LeaguesDbContext dbContext) : IGameLeaguesRepository
{
    public async Task<IReadOnlyList<int>> GetLeagueVersionIdsAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken) =>
        await dbContext.GameLeagues
            .AsNoTracking()
            .Where(league => league.UserId == userId && league.GameId == gameId)
            .Select(league => league.LeagueVersionId)
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<IReadOnlyDictionary<Guid, int>> GetLeagueVersionIdsByGameLeagueAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken) =>
        await dbContext.GameLeagues
            .AsNoTracking()
            .Where(league => league.UserId == userId && league.GameId == gameId)
            .ToDictionaryAsync(
                league => league.Id,
                league => league.LeagueVersionId,
                cancellationToken)
            .ConfigureAwait(false);

    public Task<GameLeague?> GetAsync(
        Guid userId,
        Guid gameId,
        Guid seasonId,
        int leagueVersionId,
        CancellationToken cancellationToken) =>
        dbContext.GameLeagues
            .Include(league => league.GameLeagueTeams)
            .Include(league => league.GameLeagueMatches)
            .SingleOrDefaultAsync(league =>
                league.UserId == userId
                && league.GameId == gameId
                && league.SeasonId == seasonId
                && league.LeagueVersionId == leagueVersionId,
                cancellationToken);

    public Task<GameLeague?> GetAsync(
        Guid userId,
        Guid gameLeagueId,
        CancellationToken cancellationToken) =>
        dbContext.GameLeagues
            .Include(gl => gl.GameLeagueTeams)
            .Include(gl => gl.GameLeagueMatches)
            .SingleOrDefaultAsync(gl => gl.UserId == userId && gl.Id == gameLeagueId, cancellationToken);

    public async Task<IReadOnlyList<GameLeagueMatch>> GetMatchesForDateAsync(
        Guid userId,
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        return await dbContext.GameLeagueMatches
            .Join(
                dbContext.GameLeagues,
                match => match.GameLeagueId,
                league => league.Id,
                (match, league) => new { match, league })
            .Where(item =>
                item.league.UserId == userId
                && item.league.GameId == gameId
                && item.match.Date >= startDate
                && item.match.Date < endDate)
            .Select(item => item.match)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

}
