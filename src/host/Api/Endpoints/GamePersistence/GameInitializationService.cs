using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;
using ProjectFootballSim.GamePersistence.Contracts.Features.CreateGame;

namespace ProjectFootballSim.Api.Endpoints.GamePersistence;

internal sealed class GameInitializationService(
    ICreateGameCommandHandler createGameCommandHandler,
    CreateGameCalendarCommandHandler createGameCalendarCommandHandler,
    CreateGameSeasonCommandHandler createGameSeasonCommandHandler)
{
    public async Task<CreateGameResponse> InitAsync(Guid userId, CancellationToken cancellationToken)
    {
        var createGameCommand = new CreateGameCommand(UserId: userId);
        Guid gameId = await createGameCommandHandler.HandleAsync(createGameCommand, cancellationToken).ConfigureAwait(false);

        var createGameCalendarCommand = new CreateGameCalendarCommand(userId, gameId);
        await createGameCalendarCommandHandler.HandleAsync(createGameCalendarCommand, cancellationToken).ConfigureAwait(false);

        var createGameSeasonCommand = new CreateGameSeasonCommand(userId, gameId);
        CreateGameSeasonCommandResult season = await createGameSeasonCommandHandler.HandleAsync(createGameSeasonCommand, cancellationToken).ConfigureAwait(false);

        return new CreateGameResponse(GameId: gameId, FirstSeasonId: season.Id);
    }
}
