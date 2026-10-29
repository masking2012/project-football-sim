using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueStandings;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class GetGameLeagueStandingsRepository(LeaguesDbContext dbContext) : IGetGameLeagueStandingsRepository
{
    public async Task<IReadOnlyList<GameLeagueTeamDto>> GetAsync(
        Guid userId,
        Guid gameId,
        Guid seasonId,
        int leagueVersionId,
        CancellationToken cancellationToken)
    {
        List<GameLeagueTeam> teams = await dbContext
            .GameLeagues.AsNoTracking()
            .Where(gl => gl.UserId == userId && gl.GameId == gameId && gl.SeasonId == seasonId && gl.LeagueVersionId == leagueVersionId)
            .Join(dbContext.GameLeagueTeams.AsNoTracking(), gl => gl.Id, glt => glt.GameLeagueId, (gl, glt) => glt)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        if (teams is null || teams.Count == 0)
            throw new InvalidOperationException($"No league teams were found for GameId '{gameId}', SeasonId '{seasonId}', and LeagueVersionId '{leagueVersionId}'");

        return teams
            .Select(t => new GameLeagueTeamDto(
                TeamId: t.TeamId,
                Wins: t.Wins,
                Draws: t.Draws,
                Losses: t.Losses,
                GoalsFor: t.GoalsFor,
                GoalsAgainst: t.GoalsAgainst,
                Points: t.Points
            ))
            .ToList();
    }
}
