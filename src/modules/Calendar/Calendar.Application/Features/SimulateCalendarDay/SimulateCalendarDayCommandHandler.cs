using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application.Features.SimulateCalendarDay;

internal sealed class SimulateCalendarDayCommandHandler(
    IGameCalendarWriteRepository repository,
    IEventPublisher eventPublisher)
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

        await eventPublisher.PublishAsync(new SimulateCalendarDayEvent(
            UserId: command.UserId,
            GameId: command.GameId,
            Date: gameCalendar.CurrentDate), cancellationToken).ConfigureAwait(false);

        //TODO: once fully asynchronous, this should be done once all events have been processed
        gameCalendar.UpdateDayStatus(CalendarDayStatus.Completed);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
