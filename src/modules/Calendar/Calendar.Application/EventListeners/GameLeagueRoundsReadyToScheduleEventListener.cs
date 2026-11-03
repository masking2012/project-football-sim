using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.Events;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application.EventListeners;

internal sealed class GameLeagueRoundsReadyToScheduleEventListener : IEventListener<GameLeagueRoundsReadyToScheduleEvent>
{
    public Task HandleAsync(GameLeagueRoundsReadyToScheduleEvent @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
