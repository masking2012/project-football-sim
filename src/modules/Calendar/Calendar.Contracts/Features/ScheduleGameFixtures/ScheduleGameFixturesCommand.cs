namespace ProjectFootballSim.Calendar.Contracts.Features.ScheduleGameFixtures;

public sealed record ScheduleGameFixturesCommand(
    Guid UserId,
    Guid GameId,
    IReadOnlyList<ScheduleGameFixturesRoundItem> Rounds);

public sealed record ScheduleGameFixturesRoundItem(
    Guid GameCompetitionId,
    CompetitionType CompetitionType,
    int RoundId,
    DateTime BaseDate,
    IReadOnlySet<DayOfWeek> AvailableDaysOfWeek);
