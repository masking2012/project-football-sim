using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.GamePersistence.Application.Features.CreateGame;
using ProjectFootballSim.Seasons.Application.Features.CreateGameSeason;

namespace ProjectFootballSim.Api.Endpoints.GamePersistence;

internal sealed class GameInitializationService(
    CreateGameCommandHandler createGameCommandHandler,
    CreateGameCalendarCommandHandler createGameCalendarCommandHandler,
    CreateGameSeasonCommandHandler createGameSeasonCommandHandler)
{
    public async Task InitAsync(Guid userId, CancellationToken cancellationToken)
    {
        var createGameCommand = new CreateGameCommand(UserId: userId);
        Guid gameId = createGameCommandHandler.Handle(createGameCommand);

        var createGameCalendarCommand = new CreateGameCalendarCommand(userId, gameId);
        await createGameCalendarCommandHandler.HandleAsync(createGameCalendarCommand, cancellationToken).ConfigureAwait(false);

        var createGameSeasonCommand = new CreateGameSeasonCommand(userId, gameId);
        await createGameSeasonCommandHandler.HandleAsync(createGameSeasonCommand, cancellationToken).ConfigureAwait(false);
    }
}
