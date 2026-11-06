namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.UpdateGameLeagueRoundStatus;

public interface IUpdateGameLeagueRoundStatusCommandHandler
{
    Task HandleAsync(UpdateGameLeagueRoundStatusCommand command, CancellationToken cancellationToken);
}
