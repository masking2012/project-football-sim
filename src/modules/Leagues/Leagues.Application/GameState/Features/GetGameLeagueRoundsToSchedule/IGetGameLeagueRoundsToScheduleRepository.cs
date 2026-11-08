namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueRoundsToSchedule;

public interface IGetGameLeagueRoundsToScheduleRepository
{
    Task<IReadOnlyList<ProvisionalRoundDto>> GetProvisionalRoundsAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken);
}
