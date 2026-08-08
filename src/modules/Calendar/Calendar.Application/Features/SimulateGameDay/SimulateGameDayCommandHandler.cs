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

        if (gameCalendar.State == Domain.Enums.DayState.Completed)
            throw new InvalidOperationException("Cannot simulate a game day that is already completed.");

        //TODO: implement the logic

        gameCalendar.UpdateState(Domain.Enums.DayState.Completed);

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
