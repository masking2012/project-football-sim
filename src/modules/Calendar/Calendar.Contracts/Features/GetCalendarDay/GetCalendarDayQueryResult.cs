namespace ProjectFootballSim.Calendar.Contracts.Features.GetCalendarDay;

public sealed record GetCalendarDayQueryResult(
    DateTime Date,
    string DayStatus,
    IReadOnlyList<LeagueMatchDto> LeagueMatches);
