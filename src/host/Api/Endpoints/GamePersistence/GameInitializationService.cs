using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;
using ProjectFootballSim.GamePersistence.Application.Features.CreateGame;
using ProjectFootballSim.Leagues.Application.Features.GetLeagues;
using ProjectFootballSim.Leagues.Application.GameFeatures.CreateGameLeague;

namespace ProjectFootballSim.Api.Endpoints.GamePersistence;

internal sealed class GameInitializationService(
    CreateGameCommandHandler createGameCommandHandler,
    CreateGameCalendarCommandHandler createGameCalendarCommandHandler,
    CreateGameSeasonCommandHandler createGameSeasonCommandHandler,
    GetLeaguesQueryHandler getLeaguesQueryHandler,
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

        var leagues = await getLeaguesQueryHandler.HandleAsync(cancellationToken).ConfigureAwait(false);
        foreach(var league in leagues)
        {
            await createGameLeagueCommandHandler
                .HandleAsync(new CreateGameLeagueCommand(
                    LeagueId: league.Key,
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
