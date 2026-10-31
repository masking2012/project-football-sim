namespace ProjectFootballSim.Calendar.Contracts.Features.GetCalendarDay;

public interface IGetCalendarDayQueryHandler
{
    Task<GetCalendarDayQueryResult> HandleAsync(GetCalendarDayQuery query, CancellationToken cancellationToken);
}
