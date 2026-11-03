using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueRoundsToSchedule;

namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.Events;

public sealed record GameLeagueRoundsReadyToScheduleEvent(
    Guid UserId,
    Guid GameId,
    DateTime CurrentDate,
    IReadOnlyList<GameLeagueRoundDto> Rounds);
