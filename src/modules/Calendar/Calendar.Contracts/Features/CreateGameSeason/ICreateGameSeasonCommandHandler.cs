namespace ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;

public interface ICreateGameSeasonCommandHandler
{
    Task<CreateGameSeasonCommandResult> HandleAsync(
        CreateGameSeasonCommand command,
        CancellationToken cancellationToken);
}
