namespace ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;

public interface ICreateGameCalendarRepository
{
    Task CreateAndSaveAsync(Guid userId, Guid gameId, DateTime initialDate, CancellationToken cancellationToken);
}
