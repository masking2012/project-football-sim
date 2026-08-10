namespace ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;

public sealed record CalendarDayDto(
    DateTime Date,
    string DayStatus,
    IEnumerable<LeagueMatchDto> LeagueMatches);
