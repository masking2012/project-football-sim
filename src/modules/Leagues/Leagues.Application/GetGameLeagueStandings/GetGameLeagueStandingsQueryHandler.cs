using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GetGameLeagueStandings;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GetLeaguesByCountries;

public sealed class GetGameLeagueStandingsQueryHandler(LeaguesDbContext dbContext)
{
    public async Task<IReadOnlyList<GameLeagueTeamDto>> HandleAsync(GetGameLeagueStandingsQuery query, CancellationToken cancellationToken)
    {
        var league = await dbContext.GameLeagues
            .Where(league => league.UserId == query.UserId
                && league.GameId == query.GameId
                && league.LeagueId == query.LeagueId)
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
