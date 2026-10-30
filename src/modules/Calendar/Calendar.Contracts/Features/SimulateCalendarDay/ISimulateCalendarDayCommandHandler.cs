namespace ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;

public interface ISimulateCalendarDayCommandHandler
{
    Task HandleAsync(SimulateCalendarDayCommand command, CancellationToken cancellationToken);
}
