using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;
using ProjectFootballSim.GamePersistence.Application.Features.CreateGame;
using ProjectFootballSim.Leagues.Application.Catalog.Features.GetActiveLeagues;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.GameState.Features.CreateGameLeague;

namespace ProjectFootballSim.Api.Endpoints.GamePersistence;

internal sealed class GameInitializationService(
    CreateGameCommandHandler createGameCommandHandler,
    CreateGameCalendarCommandHandler createGameCalendarCommandHandler,
    CreateGameSeasonCommandHandler createGameSeasonCommandHandler,
    GetActiveLeaguesQueryHandler getActiveLeaguesQueryHandler,
    CreateGameLeagueCommandHandler createGameLeagueCommandHandler)
{
    public async Task<CreateGameResponse> InitAsync(Guid userId, CancellationToken cancellationToken)
    {
        var createGameCommand = new CreateGameCommand(UserId: userId);
        Guid gameId = createGameCommandHandler.Handle(createGameCommand);

        var createGameCalendarCommand = new CreateGameCalendarCommand(userId, gameId);
        await createGameCalendarCommandHandler.HandleAsync(createGameCalendarCommand, cancellationToken).ConfigureAwait(false);

        var createGameSeasonCommand = new CreateGameSeasonCommand(userId, gameId);
        CreateGameSeasonCommandResult season = await createGameSeasonCommandHandler.HandleAsync(createGameSeasonCommand, cancellationToken).ConfigureAwait(false);

        IReadOnlyDictionary<int, LeagueDto> activeLeagues
            = await getActiveLeaguesQueryHandler.HandleAsync(cancellationToken).ConfigureAwait(false);
        foreach(var league in activeLeagues)
        {
            //TODO: send LeagueVersionId instead of LeagueId
            await createGameLeagueCommandHandler
                .HandleAsync(new CreateGameLeagueCommand(
                    LeagueVersionId: league.Key,
                    GameId: gameId,
                    UserId: userId,
                    SeasonId: season.Id,
                    SeasonStartDate: season.StartDate,
                    PreviousSeasonId: null), cancellationToken)
                .ConfigureAwait(false);
        }

        return new CreateGameResponse(GameId: gameId, FirstSeasonId: season.Id);
    }
}
