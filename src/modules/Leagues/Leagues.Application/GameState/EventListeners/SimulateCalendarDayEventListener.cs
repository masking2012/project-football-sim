using ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.SimulateLeagueGameDay;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Leagues.Application.GameState.EventListeners;

internal sealed class SimulateCalendarDayEventListener(
    ISimulateLeagueGameDayCommandHandler simulateLeagueGameDayCommandHandler)
    : IEventListener<SimulateCalendarDayEvent>
{
    public async Task HandleAsync(SimulateCalendarDayEvent @event, CancellationToken cancellationToken)
    {
        await simulateLeagueGameDayCommandHandler
            .HandleAsync(
                new SimulateLeagueGameDayCommand(
                    @event.UserId,
                    @event.GameId,
                    @event.Date),
                    cancellationToken)
            .ConfigureAwait(false);
    }
}
