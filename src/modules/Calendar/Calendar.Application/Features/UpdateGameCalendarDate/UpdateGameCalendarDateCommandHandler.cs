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
            .SingleOrDefaultAsync(x => x.GameId == command.GameId, cancellationToken)
            .ConfigureAwait(false);

        if (gameCalendar is null)
            throw new InvalidOperationException("Game calendar not found for the specified game ID.");

        gameCalendar.UpdateDate(command.NewDate);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
