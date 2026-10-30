namespace ProjectFootballSim.GamePersistence.Contracts.Features.CreateGame;

public interface ICreateGameCommandHandler
{
    Task<Guid> HandleAsync(CreateGameCommand command, CancellationToken cancellationToken);
}
