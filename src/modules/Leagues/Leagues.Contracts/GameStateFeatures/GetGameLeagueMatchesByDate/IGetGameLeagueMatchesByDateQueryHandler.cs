namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByDate;

public interface IGetGameLeagueMatchesByDateQueryHandler
{
    Task<IReadOnlyList<GetGameLeagueMatchDto>> HandleAsync(
        GetGameLeagueMatchesByDateQuery query,
        CancellationToken cancellationToken);
}
