namespace ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;

public sealed record SimulateCalendarDayEvent(
    Guid UserId,
    Guid GameId,
    DateTime Date);
