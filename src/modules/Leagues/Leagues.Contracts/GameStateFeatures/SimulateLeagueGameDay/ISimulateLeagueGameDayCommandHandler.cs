namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.SimulateLeagueGameDay;

public interface ISimulateLeagueGameDayCommandHandler
{
    Task HandleAsync(SimulateLeagueGameDayCommand command, CancellationToken cancellationToken);
}
