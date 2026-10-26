using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueFixtures;

public sealed class GetGameLeagueFixturesQueryHandler(LeaguesDbContext dbContext)
{
    public async Task<IReadOnlyList<GameLeagueMatchDto>> HandleAsync(
        GetGameLeagueFixturesQuery query,
        CancellationToken cancellationToken)
    {
        var league = await dbContext.GameLeagues
            .AsNoTracking()
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                gameLeague => gameLeague.LeagueVersionId,
                version => version.LeagueVersionId,
                (gameLeague, version) => new { gameLeague, version })
            .Where(f =>
                f.gameLeague.UserId == query.UserId
                && f.version.LeagueId == query.LeagueId
                && f.gameLeague.GameId == query.GameId
                && f.gameLeague.SeasonId == query.SeasonId)
            .Select(f => f.gameLeague)
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
