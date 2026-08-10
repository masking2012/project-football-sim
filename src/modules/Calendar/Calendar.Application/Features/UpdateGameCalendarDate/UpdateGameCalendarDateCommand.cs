namespace ProjectFootballSim.Calendar.Application.Features.UpdateGameCalendarDate;

public sealed record UpdateGameCalendarDateCommand(
    Guid UserId,
    Guid GameId,
    DateTime NewDate);
