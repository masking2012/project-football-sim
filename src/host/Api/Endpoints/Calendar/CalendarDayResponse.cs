namespace ProjectFootballSim.Api.Endpoints.Calendar;

internal sealed record CalendarDayResponse(
    DateTime Date,
    string DayStatus,
    IEnumerable<LeagueMatchResponse> LeagueMatches);
