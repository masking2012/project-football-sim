using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;

public sealed class CreateGameCalendarCommandHandler(CalendarDbContext dbContext)
{
    public async Task HandleAsync(
        CreateGameCalendarCommand command,
        CancellationToken cancellationToken)
    {
        var gameCalendar = new GameCalendar(command.UserId, command.GameId, command.NewDate);
        dbContext.GameCalendars.Add(gameCalendar);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
