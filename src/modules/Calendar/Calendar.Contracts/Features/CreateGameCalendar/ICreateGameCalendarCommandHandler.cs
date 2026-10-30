namespace ProjectFootballSim.Calendar.Contracts.Features.CreateGameCalendar;

public interface ICreateGameCalendarCommandHandler
{
    Task HandleAsync(CreateGameCalendarCommand command, CancellationToken cancellationToken);
}
