using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Leagues.Application.GameFeatures.SimulateLeagueGameDay;

namespace ProjectFootballSim.Calendar.Application.Features.SimulateCalendarDay;

public sealed class SimulateCalendarDayCommandHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    CalendarDbContext dbContext,
    SimulateLeagueGameDayCommandHandler simulateLeagueGameDayCommandHandler)
{
    public async Task HandleAsync(SimulateCalendarDayCommand command, CancellationToken cancellationToken)
    {
        var gameCalendar = await gameCalendarRetriever
            .GetCurrentGameDateAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);

        if (gameCalendar.DayStatus == CalendarDayStatus.Completed)
            throw new InvalidOperationException("Cannot simulate a game day that is already completed.");

        DateTime currentDate = gameCalendar.CurrentDate;
        await simulateLeagueGameDayCommandHandler
            .HandleAsync(
                new SimulateLeagueGameDayCommand(
                    command.UserId,
                    command.GameId,
                    currentDate),
                cancellationToken)
            .ConfigureAwait(false);

        gameCalendar.UpdateDayStatus(CalendarDayStatus.Completed);

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
