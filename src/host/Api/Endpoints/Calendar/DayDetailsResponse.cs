using ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;

namespace ProjectFootballSim.Api.Endpoints.Calendar;

internal sealed record DayDetailsResponse(
    DateTime Date,
    string DayState,
    IEnumerable<MatchEventResponse> MatchEvents);
