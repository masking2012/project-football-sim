namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.CheckGameLeagueMatchesByDate;

public interface ICheckGameLeagueMatchesByDateQueryHandler
{
    Task<CheckGameLeagueMatchesByDateQueryResult> HandleAsync(
        CheckGameLeagueMatchesByDateQuery query,
        CancellationToken cancellationToken);
}
