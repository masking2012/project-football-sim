namespace ProjectFootballSim.Calendar.Contracts.Features.ScheduleGameFixtures;

public sealed record ScheduleGameLeagueFixturesEvent(
    Guid UserId,
    Guid GameId,
    IReadOnlyList<ScheduledLeagueVersion> LeagueVersions);

public sealed record ScheduledLeagueVersion(
    int LeagueVersionId,
    int Round,
    IReadOnlyList<ScheduledLeagueMatch> Matches);

public sealed record ScheduledLeagueMatch(
    Guid MatchId,
    DateTime Date);
