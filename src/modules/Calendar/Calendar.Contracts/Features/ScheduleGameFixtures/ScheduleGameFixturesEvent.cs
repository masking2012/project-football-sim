namespace ProjectFootballSim.Calendar.Contracts.Features.ScheduleGameFixtures;

public sealed record ScheduleGameFixturesEvent(
    Guid UserId,
    Guid GameId,
    IReadOnlyList<ScheduledVersionEventItem> Versions);

public sealed record ScheduledVersionEventItem(
    int CompetitionVersionId,
    CompetitionType CompetitionType,
    int Round,
    IReadOnlyList<ScheduledMatchEventItem> Matches);

public sealed record ScheduledMatchEventItem(
    Guid MatchId,
    DateTime Date);
