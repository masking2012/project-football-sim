namespace ProjectFootballSim.Calendar.Contracts.Features.GetCalendarDay;

public sealed record GetCalendarDayQuery(
    Guid UserId,
    Guid GameId,
    DateTime? Date);
