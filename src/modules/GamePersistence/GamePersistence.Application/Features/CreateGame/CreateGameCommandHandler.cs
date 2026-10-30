using Microsoft.Extensions.Logging;
using ProjectFootballSim.GamePersistence.Contracts.Events;
using ProjectFootballSim.GamePersistence.Contracts.Features.CreateGame;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.GamePersistence.Application.Features.CreateGame;

internal sealed class CreateGameCommandHandler(
    ILogger<CreateGameCommandHandler> logger,
    IEventPublisher eventPublisher)
    : ICreateGameCommandHandler
{
    public async Task<Guid> HandleAsync(CreateGameCommand command, CancellationToken cancellationToken)
    {
        var gameId = Guid.NewGuid();
        CreateGameLogs.NewGameCreated(logger, command.UserId, gameId);

        await eventPublisher.PublishAsync(new GameCreatedEvent(
            UserId: command.UserId,
            GameId: gameId), cancellationToken).ConfigureAwait(false);

        return gameId;
    }
}
