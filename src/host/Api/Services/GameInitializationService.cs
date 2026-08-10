using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.GamePersistence.Application.Features.CreateGame;

namespace ProjectFootballSim.Api.Services;

internal sealed class GameInitializationService(
    CreateGameCommandHandler createGameCommandHandler,
    CreateGameCalendarCommandHandler createGameCalendarCommandHandler)
{
    public async Task InitAsync(Guid userId, CancellationToken cancellationToken)
    {
        var createGameCommand = new CreateGameCommand(UserId: userId);
        Guid gameId = createGameCommandHandler.Handle(createGameCommand);

        var createGameCalendarCommand = new CreateGameCalendarCommand(userId, gameId);
        await createGameCalendarCommandHandler.HandleAsync(createGameCalendarCommand, cancellationToken).ConfigureAwait(false);
    }
}
