namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueRoundsToSchedule;

public sealed record GameLeagueRoundDto(
    Guid GameLeagueId,
    int LeagueVersionId,
    int Round,
    DateTime BaseDate,
    HashSet<DayOfWeek> MatchDays);
