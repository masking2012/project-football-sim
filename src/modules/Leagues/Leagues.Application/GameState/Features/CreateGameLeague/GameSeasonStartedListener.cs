using ProjectFootballSim.Calendar.Contracts;
using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Contracts.CatalogFeatures.GetActiveLeagues;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.CreateGameLeague;

public sealed class GameSeasonStartedListener(
    IGetActiveLeaguesQueryHandler getActiveLeaguesQueryHandler,
    CreateGameLeagueCommandHandler createGameLeagueCommandHandler,
    IGameLeagueWriteRepository writeRepository) : IEventListener<GameSeasonStartedEvent>
{
    public async Task HandleAsync(GameSeasonStartedEvent @event, CancellationToken cancellationToken)
    {
        var activeLeagues = await getActiveLeaguesQueryHandler.HandleAsync(cancellationToken).ConfigureAwait(false);

        foreach (int leagueVersionId in activeLeagues.Values.Select(league => league.LeagueVersionId))
        {
            bool exists = await writeRepository
                .ExistsAsync(@event.GameId, @event.SeasonId, leagueVersionId, cancellationToken)
                .ConfigureAwait(false);
            if (exists)
                continue;

            await createGameLeagueCommandHandler.HandleAsync(new CreateGameLeagueCommand(
                UserId: @event.UserId,
                GameId: @event.GameId,
                SeasonId: @event.SeasonId,
                SeasonStartDate: @event.StartDate,
                PreviousSeasonId: @event.PreviousSeasonId,
                LeagueVersionId: leagueVersionId), cancellationToken).ConfigureAwait(false);
        }
    }
}
