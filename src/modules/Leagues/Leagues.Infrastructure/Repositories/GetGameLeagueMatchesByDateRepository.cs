using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class GetGameLeagueMatchesByDateRepository(LeaguesDbContext dbContext) : IGetGameLeagueMatchesByDateRepository
{
    //TODO: do we need to accept "SeasonId" as a parameter here?
    public async Task<IReadOnlyList<LeagueMatchDto>> GetAsync(Guid userId, Guid gameId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var matches = await dbContext.GameLeagues
            .AsNoTracking()
            .Where(gl => gl.UserId == userId && gl.GameId == gameId)
            .Join(dbContext.GameLeagueMatches.AsNoTracking(),
                league => league.Id,
                match => match.GameLeagueId,
                (league, match) => new { match, league.LeagueVersionId })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return matches
            .Select(m => new LeagueMatchDto(
                Id: m.match.Id,
                Date: m.match.Date,
                HomeTeamId: m.match.HomeTeamId,
                AwayTeamId: m.match.AwayTeamId,
                HomeTeamScore: m.match.HomeTeamScore,
                AwayTeamScore: m.match.AwayTeamScore,
                LeagueRound: m.match.Round,
                LeagueVersionId: m.LeagueVersionId))
            .ToList();
    }
}
