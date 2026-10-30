namespace ProjectFootballSim.Calendar.Contracts.Features.AdvanceCalendarDay;

public interface IAdvanceCalendarDayCommandHandler
{
    Task HandleAsync(AdvanceCalendarDayCommand command, CancellationToken cancellationToken);
}
