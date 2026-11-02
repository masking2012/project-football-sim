using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.Events;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application.EventListeners;

internal sealed class GameLeagueMatchesProvisionalEventListener : IEventListener<GameLeagueMatchesProvisionalEvent>
{
    public Task HandleAsync(GameLeagueMatchesProvisionalEvent @event, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
