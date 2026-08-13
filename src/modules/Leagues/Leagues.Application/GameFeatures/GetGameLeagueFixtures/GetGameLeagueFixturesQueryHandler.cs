using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueFixtures;

public sealed class GetGameLeagueFixturesQueryHandler(LeaguesDbContext dbContext)
{
    public async Task<IReadOnlyList<GameLeagueMatchDto>> HandleAsync(
        GetGameLeagueFixturesQuery query,
        CancellationToken cancellationToken)
    {
        var league = await dbContext.GameLeagues
            .AsNoTracking()
            .Where(f =>
                f.UserId == query.UserId
                && f.LeagueId == query.LeagueId
                && f.GameId == query.GameId
                && f.SeasonId == query.SeasonId)
            .Include(f => f.GameLeagueMatches)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (league is null)
            throw new InvalidOperationException("League not found");

        return league.GameLeagueMatches
            .Select(f => new GameLeagueMatchDto(
                Id: f.Id,
                Date: f.Date,
                HomeTeamId: f.HomeTeamId,
                AwayTeamId: f.AwayTeamId,
                HomeTeamScore: f.HomeTeamScore,
                AwayTeamScore: f.AwayTeamScore,
                Round: f.Round))
            .ToList();
    }
}
