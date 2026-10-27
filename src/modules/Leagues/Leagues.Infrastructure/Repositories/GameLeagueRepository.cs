using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Models;
using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class GameLeagueRepository(LeaguesDbContext dbContext) : IGameLeagueRepository
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

    public async Task<IReadOnlyList<GameLeagueMatchByDateDto>> GetMatchesByDateAsync(
        Guid userId,
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        return await dbContext.GameLeagueMatches
            .AsNoTracking()
            .Join(
                dbContext.GameLeagues.AsNoTracking(),
                match => match.GameLeagueId,
                league => league.Id,
                (match, league) => new { match, league })
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                item => item.league.LeagueVersionId,
                version => version.LeagueVersionId,
                (item, version) => new { item.match, item.league, version })
            .Where(item =>
                item.league.UserId == userId
                && item.league.GameId == gameId
                && item.match.Date >= startDate
                && item.match.Date < endDate)
            .Select(item => new GameLeagueMatchByDateDto(item.match, item.version.LeagueVersionId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
