using ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;
using ProjectFootballSim.Leagues.Contracts.CatalogFeatures.GetActiveLeagues;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.CreateGameLeague;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Leagues.Application.GameState.EventListeners;

internal sealed class GameSeasonStartedEventListener(
    IGetActiveLeaguesQueryHandler getActiveLeaguesQueryHandler,
    ICreateGameLeagueCommandHandler createGameLeagueCommandHandler)
    : IEventListener<GameSeasonStartedEvent>
{
    public async Task HandleAsync(GameSeasonStartedEvent @event, CancellationToken cancellationToken)
    {
        var activeLeagues = await getActiveLeaguesQueryHandler.HandleAsync(cancellationToken).ConfigureAwait(false);

        foreach (int leagueVersionId in activeLeagues.Values.Select(league => league.LeagueVersionId))
        {
            var command = new CreateGameLeagueCommand(
                UserId: @event.UserId,
                GameId: @event.GameId,
                SeasonId: @event.SeasonId,
                SeasonStartDate: @event.StartDate,
                PreviousSeasonId: @event.PreviousSeasonId,
                LeagueVersionId: leagueVersionId);
            await createGameLeagueCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}
