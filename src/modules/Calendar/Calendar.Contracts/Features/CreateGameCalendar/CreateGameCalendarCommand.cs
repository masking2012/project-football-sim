namespace ProjectFootballSim.Calendar.Contracts.Features.CreateGameCalendar;

public sealed record CreateGameCalendarCommand(
    Guid UserId,
    Guid GameId);
