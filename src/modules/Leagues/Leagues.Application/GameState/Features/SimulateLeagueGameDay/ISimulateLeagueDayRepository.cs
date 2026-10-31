using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.SimulateLeagueGameDay;

public interface ISimulateLeagueDayRepository
{
    Task<IReadOnlyList<GameLeague>> GetLeaguesWithMatchesForDateAsync(
        Guid userId, Guid gameId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
