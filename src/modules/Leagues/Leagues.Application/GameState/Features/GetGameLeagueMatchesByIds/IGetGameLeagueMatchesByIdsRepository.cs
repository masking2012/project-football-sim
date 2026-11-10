namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByIds;

public interface IGetGameLeagueMatchesByIdsRepository
{
    Task<IReadOnlyList<LeagueMatchDto>> GetAsync(
        Guid userId,
        Guid gameId,
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken);
}
