namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueRoundsToSchedule;

public sealed record GetGameLeagueRoundsToScheduleQuery(
    Guid UserId,
    Guid GameId,
    DateTime CurrentDate);
