using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.UpdateGameLeagueMatches;

public interface IUpdateGameLeagueMatchesRepository
{
    Task<IReadOnlyList<GameLeagueMatch>> GetMatchesAsync(
        Guid userId,
        Guid gameId,
        IReadOnlyList<Guid> matchIds,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
