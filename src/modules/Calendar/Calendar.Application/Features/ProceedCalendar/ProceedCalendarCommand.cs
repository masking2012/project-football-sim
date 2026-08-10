namespace ProjectFootballSim.Calendar.Application.Features.ProceedCalendar;

public sealed record ProceedCalendarCommand(
    Guid UserId,
    Guid GameId);
