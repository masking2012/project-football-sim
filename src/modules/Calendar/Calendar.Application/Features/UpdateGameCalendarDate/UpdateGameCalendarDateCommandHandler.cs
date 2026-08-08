using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Application.Features.UpdateGameCalendarDate;

public sealed class UpdateGameCalendarDateCommandHandler(CalendarDbContext dbContext)
{
    public async Task HandleAsync(
        UpdateGameCalendarDateCommand command,
        CancellationToken cancellationToken)
    {
        GameCalendar? gameCalendar = await dbContext.GameCalendars
            .SingleOrDefaultAsync(x => x.UserId == command.UserId && x.GameId == command.GameId, cancellationToken)
            .ConfigureAwait(false);

        if (gameCalendar is null)
            throw new InvalidOperationException($"Game calendar not found for UserId: {command.UserId}, GameId: {command.GameId}");

        gameCalendar.UpdateDate(command.NewDate);
        gameCalendar.UpdateState(Domain.Enums.DayState.NotStarted);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
