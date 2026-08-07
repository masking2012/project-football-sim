namespace ProjectFootballSim.Calendar.Application.Features.UpdateGameCalendarDate;

public sealed record UpdateGameCalendarDateCommand(Guid GameId, DateTime NewDate);
