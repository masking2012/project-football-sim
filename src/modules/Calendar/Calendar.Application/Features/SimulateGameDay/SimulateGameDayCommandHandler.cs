using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Leagues.Application.GameFeatures.SimulateLeagueGameDay;

namespace ProjectFootballSim.Calendar.Application.Features.SimulateGameDay;

public sealed class SimulateGameDayCommandHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    CalendarDbContext dbContext,
    SimulateLeagueGameDayCommandHandler simulateLeagueGameDayCommandHandler)
{
    public async Task HandleAsync(SimulateGameDayCommand command, CancellationToken cancellationToken)
    {
        var gameCalendar = await gameCalendarRetriever
            .GetCurrentGameDateAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);

        if (gameCalendar.State == Domain.Enums.DayState.Completed)
            throw new InvalidOperationException("Cannot simulate a game day that is already completed.");

        var simulateLeagueGameDayCommand = new SimulateLeagueGameDayCommand(command.UserId, command.GameId, gameCalendar.CurrentDate);
        await simulateLeagueGameDayCommandHandler.HandleAsync(simulateLeagueGameDayCommand, cancellationToken).ConfigureAwait(false);

        gameCalendar.UpdateState(Domain.Enums.DayState.Completed);

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
