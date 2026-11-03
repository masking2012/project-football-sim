using ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.Events;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueRoundsToSchedule;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.SimulateLeagueGameDay;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Leagues.Application.GameState.EventListeners;

internal sealed class SimulateCalendarDayEventListener(
    ISimulateLeagueGameDayCommandHandler simulateLeagueGameDayCommandHandler,
    IGetGameLeagueRoundsToScheduleQueryHandler getGameLeagueRoundsToScheduleQueryHandler,
    IEventPublisher eventPublisher)
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

        var provisionalRounds = await getGameLeagueRoundsToScheduleQueryHandler.HandleAsync(new GetGameLeagueRoundsToScheduleQuery(
            @event.UserId,
            @event.GameId,
            @event.Date),
            cancellationToken)
            .ConfigureAwait(false);
        if (provisionalRounds.Count > 0)
        {
            await eventPublisher.PublishAsync(new GameLeagueRoundsReadyToScheduleEvent(
                    @event.UserId,
                    @event.GameId,
                    @event.Date,
                    provisionalRounds),
                cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
