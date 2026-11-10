namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.UpdateGameLeagueMatches;

public interface IUpdateGameLeagueMatchesCommandHandler
{
    Task HandleAsync(UpdateGameLeagueMatchesCommand command, CancellationToken cancellationToken);
}
