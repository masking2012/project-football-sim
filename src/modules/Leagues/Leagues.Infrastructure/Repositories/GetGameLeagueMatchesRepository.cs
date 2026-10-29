using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueFixtures;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class GetGameLeagueMatchesRepository(LeaguesDbContext dbContext) : IGetGameLeagueMatchesRepository
{
    public async Task<IReadOnlyList<GameLeagueMatchDto>> GetAsync(
        Guid userId,
        Guid gameId,
        Guid seasonId,
        int leagueVersionId,
        CancellationToken cancellationToken)
    {
        List<GameLeagueMatch> matches = await dbContext
            .GameLeagues.AsNoTracking()
            .Where(gl => gl.UserId == userId && gl.GameId == gameId && gl.SeasonId == seasonId && gl.LeagueVersionId == leagueVersionId)
            .Join(dbContext.GameLeagueMatches.AsNoTracking(), gl => gl.Id, glt => glt.GameLeagueId, (gl, glt) => glt)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        if (matches is null || matches.Count == 0)
            throw new InvalidOperationException($"No league matches were found for GameId '{gameId}', SeasonId '{seasonId}', and LeagueVersionId '{leagueVersionId}'");

        return matches
            .Select(m => new GameLeagueMatchDto(
                Id: m.Id,
                HomeTeamId: m.HomeTeamId,
                AwayTeamId: m.AwayTeamId,
                HomeTeamGoals: m.HomeTeamScore,
                AwayTeamGoals: m.AwayTeamScore,
                Date: m.Date,
                Round: m.Round
            ))
            .ToList();
    }
}
