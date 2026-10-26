using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueStandings;

public sealed class GetGameLeagueStandingsQueryHandler(LeaguesDbContext dbContext)
{
    public async Task<IReadOnlyList<GameLeagueTeamDto>> HandleAsync(
        GetGameLeagueStandingsQuery query,
        CancellationToken cancellationToken)
    {
        var league = await dbContext.GameLeagues
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                gameLeague => gameLeague.LeagueVersionId,
                version => version.LeagueVersionId,
                (gameLeague, version) => new { gameLeague, version })
            .Where(league => league.gameLeague.UserId == query.UserId
                && league.gameLeague.GameId == query.GameId
                && league.gameLeague.SeasonId == query.SeasonId
                && league.version.LeagueId == query.LeagueId)
            .Select(league => league.gameLeague)
            .Include(league => league.GameLeagueTeams)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (league is null)
            throw new InvalidOperationException($"Game league not found for user {query.UserId}, game {query.GameId}, league {query.LeagueId}");

        return league.GameLeagueTeams
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalsFor - t.GoalsAgainst)
            .ThenByDescending(t => t.GoalsFor)
            .ThenBy(t => t.TeamId)
            .Select((t, index) => new GameLeagueTeamDto(
                Position: index + 1,
                TeamId: t.TeamId,
                Wins: t.Wins,
                Draws: t.Draws,
                Losses: t.Losses,
                GoalsFor: t.GoalsFor,
                GoalsAgainst: t.GoalsAgainst,
                Points: t.Points))
            .ToList();
    }
}
