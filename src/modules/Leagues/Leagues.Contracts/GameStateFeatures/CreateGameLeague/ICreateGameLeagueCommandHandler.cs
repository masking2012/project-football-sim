namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.CreateGameLeague;

public interface ICreateGameLeagueCommandHandler
{
    Task HandleAsync(CreateGameLeagueCommand command, CancellationToken cancellationToken);
}
