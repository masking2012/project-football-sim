using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Application.Features.ProceedCalendar;

public sealed class ProceedCalendarCommandHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    CalendarDbContext dbContext)
{
    public async Task HandleAsync(ProceedCalendarCommand command, CancellationToken cancellationToken)
    {
        GameCalendar gameCalendar = await gameCalendarRetriever
            .GetCurrentGameDateAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);

        gameCalendar.UpdateDate(gameCalendar.CurrentDate.AddDays(1));
        gameCalendar.UpdateState(Domain.Enums.DayState.NotStarted);

        // todo: check about events

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
