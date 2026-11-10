using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Features.UpdateGameLeagueRoundStatus;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class UpdateGameLeagueRoundStatusRepository(LeaguesDbContext dbContext)
    : IUpdateGameLeagueRoundStatusRepository
{
    public async Task<IReadOnlyList<GameLeagueRound>> GetRoundsAsync(
        Guid userId,
        Guid gameId,
        IReadOnlyList<(Guid GameLeagueId, int Round)> rounds,
        CancellationToken cancellationToken)
    {
        var gameLeagueIds = rounds.Select(r => r.GameLeagueId).Distinct().ToList();
        var roundNumbers = rounds.Select(r => r.Round).Distinct().ToList();

        var candidates = await dbContext.GameLeagueRounds
            .Where(round => gameLeagueIds.Contains(round.GameLeagueId) && roundNumbers.Contains(round.Round))
            .Where(round => dbContext.GameLeagues.Any(gameLeague =>
                gameLeague.Id == round.GameLeagueId && gameLeague.UserId == userId && gameLeague.GameId == gameId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return candidates
            .Where(round => rounds.Contains((round.GameLeagueId, round.Round)))
            .ToList();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
