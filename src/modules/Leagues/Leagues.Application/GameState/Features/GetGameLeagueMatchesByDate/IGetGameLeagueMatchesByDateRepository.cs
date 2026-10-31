namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;

public interface IGetGameLeagueMatchesByDateRepository
{
    Task<IReadOnlyList<LeagueMatchDto>> GetAsync(
        Guid userId,
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken);
}
