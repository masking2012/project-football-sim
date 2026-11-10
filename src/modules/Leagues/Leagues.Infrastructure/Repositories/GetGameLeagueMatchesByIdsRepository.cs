using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByIds;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class GetGameLeagueMatchesByIdsRepository(LeaguesDbContext dbContext)
    : IGetGameLeagueMatchesByIdsRepository
{
    public async Task<IReadOnlyList<LeagueMatchDto>> GetAsync(Guid userId, Guid gameId, IReadOnlyList<Guid> ids, CancellationToken cancellationToken)
    {
        var matches = await dbContext.GameLeagues
            .AsNoTracking()
            .Where(gl => gl.UserId == userId && gl.GameId == gameId)
            .Join(
                dbContext.GameLeagueMatches.AsNoTracking()
                    .Where(m => ids.Contains(m.Id)),
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
