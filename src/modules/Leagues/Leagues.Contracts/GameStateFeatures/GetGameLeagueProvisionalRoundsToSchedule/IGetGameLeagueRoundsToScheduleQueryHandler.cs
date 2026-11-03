namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueRoundsToSchedule;

public interface IGetGameLeagueRoundsToScheduleQueryHandler
{
    Task<IReadOnlyList<GameLeagueRoundDto>> HandleAsync(
        GetGameLeagueRoundsToScheduleQuery query,
        CancellationToken cancellationToken);
}
