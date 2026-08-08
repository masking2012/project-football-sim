using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Application.Features.SimulateGameDay;

public sealed class SimulateGameDayCommandHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    CalendarDbContext dbContext)
{
    public async Task HandleAsync(SimulateGameDayCommand command, CancellationToken cancellationToken)
    {
        var gameCalendar = await gameCalendarRetriever
            .GetCurrentGameDateAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);

        //TODO: implement the logic

        gameCalendar.UpdateState(Domain.Enums.DayState.Completed);

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
