using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueRoundsToSchedule;

namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.Events;

public sealed record GameLeagueRoundsReadyToScheduleEvent(
    Guid UserId,
    Guid GameId,
    DateTime CurrentDate,
    IReadOnlyList<EventGameLeagueProvisionalRound> Rounds);

public sealed record EventGameLeagueProvisionalRound(
    Guid GameLeagueId,
    int LeagueVersionId,
    int Round,
    DateTime BaseDate,
    HashSet<DayOfWeek> MatchDays);
