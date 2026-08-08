using ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;

namespace ProjectFootballSim.Calendar.Application.Features.GetDayWithEvents;

public sealed record DayDetailsDto(
    DateTime Date,
    string DayState,
    IEnumerable<MatchEventDto> MatchEvents);
