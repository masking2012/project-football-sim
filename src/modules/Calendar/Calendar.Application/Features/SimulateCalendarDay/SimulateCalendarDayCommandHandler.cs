using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Leagues.Application.GameState.Features.SimulateLeagueGameDay;

namespace ProjectFootballSim.Calendar.Application.Features.SimulateCalendarDay;

public sealed class SimulateCalendarDayCommandHandler(
    IGameCalendarWriteRepository repository,
    SimulateLeagueGameDayCommandHandler simulateLeagueGameDayCommandHandler)
    : ISimulateCalendarDayCommandHandler
{
    public async Task HandleAsync(SimulateCalendarDayCommand command, CancellationToken cancellationToken)
    {
        GameCalendar? gameCalendar = await repository
            .GetCurrentCalendarAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);
        if (gameCalendar is null)
            throw new InvalidOperationException($"Game calendar not found for user '{command.UserId}' and game '{command.GameId}'");

        if (gameCalendar.DayStatus == CalendarDayStatus.Completed)
            throw new InvalidOperationException("Cannot simulate a game day that is already completed.");

        //TODO: raise event for simulating a game day (e.g., SimulateGameDateEvent)
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
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
