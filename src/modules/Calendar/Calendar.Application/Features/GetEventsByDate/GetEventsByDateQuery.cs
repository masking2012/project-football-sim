namespace ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;

public sealed record GetEventsByDateQuery(Guid GameId, DateTime? Date);
