namespace ProjectFootballSim.Calendar.Application.Features.GetDayWithEvents;

public sealed record GetDayWithEventsQuery(
    Guid UserId,
    Guid GameId,
    DateTime? Date);
