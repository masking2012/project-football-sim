namespace ProjectFootballSim.Calendar.Application.Features.AdvanceCalendarDay;

public interface IAdvanceCalendarDayRepository
{
    Task<bool> MatchesExistAsync(Guid userId, Guid gameId, DateTime calendarDate, CancellationToken cancellationToken);}
