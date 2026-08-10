namespace ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;

public sealed record GetCalendarDayQuery(
    Guid UserId,
    Guid GameId,
    DateTime? Date);
