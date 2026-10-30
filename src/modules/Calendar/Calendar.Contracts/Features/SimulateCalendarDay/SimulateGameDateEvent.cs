namespace ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;

public sealed record SimulateGameDateEvent(
    Guid UserId,
    Guid GameId,
    DateTime Date);
