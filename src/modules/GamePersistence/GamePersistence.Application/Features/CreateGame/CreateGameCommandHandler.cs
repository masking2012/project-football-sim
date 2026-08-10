using Microsoft.Extensions.Logging;

namespace ProjectFootballSim.GamePersistence.Application.Features.CreateGame;

public sealed class CreateGameCommandHandler(ILogger<CreateGameCommandHandler> logger)
{
    public Guid Handle(CreateGameCommand command)
    {
        var gameId = Guid.NewGuid();
        Log.NewGameCreated(logger, command.UserId, gameId);
        return gameId;
    }
}
