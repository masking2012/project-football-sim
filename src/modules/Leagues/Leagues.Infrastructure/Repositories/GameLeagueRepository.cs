using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.Persistence.Models;
using ProjectFootballSim.Leagues.Application.Persistence.Repositories;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class GameLeagueRepository(LeaguesDbContext dbContext) : IGameLeagueRepository, IGameLeagueWriteRepository
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

    public async Task<IReadOnlyList<int>> GetTeamIdsAsync(
        int leagueVersionId,
        CancellationToken cancellationToken) =>
        await dbContext.LeagueTeams
            .AsNoTracking()
            .Where(team => team.LeagueVersionId == leagueVersionId)
            .Select(team => team.TeamId)
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
            .Include(league => league.GameLeagueTeams)
            .Include(league => league.GameLeagueMatches)
            .SingleOrDefaultAsync(league => league.UserId == userId && league.Id == gameLeagueId, cancellationToken);

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

    public async Task<IReadOnlyList<GameLeagueMatchByDate>> GetMatchesByDateAsync(
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
            .Select(item => new GameLeagueMatchByDate(item.match, item.version.LeagueVersionId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task AddAsync(GameLeague gameLeague, CancellationToken cancellationToken)
    {
        dbContext.GameLeagues.Add(gameLeague);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
