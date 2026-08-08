namespace ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;

public sealed record GetEventsByDateQuery(
    Guid UserId,
    Guid GameId,
    DateTime? Date);
