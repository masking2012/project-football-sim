namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByIds;

public interface IGetGameLeagueMatchesByIdsQueryHandler
{
    Task<IReadOnlyList<GetGameLeagueMatchDto>> HandleAsync(
        GetGameLeagueMatchesByIdsQuery query,
        CancellationToken cancellationToken);
}
