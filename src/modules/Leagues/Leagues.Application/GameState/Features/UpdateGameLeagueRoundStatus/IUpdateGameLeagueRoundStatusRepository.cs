using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.UpdateGameLeagueRoundStatus;

public interface IUpdateGameLeagueRoundStatusRepository
{
    Task<IReadOnlyList<GameLeagueRound>> GetRoundsAsync(
        Guid userId,
        Guid gameId,
        IReadOnlyList<(Guid GameLeagueId, int Round)> rounds,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
