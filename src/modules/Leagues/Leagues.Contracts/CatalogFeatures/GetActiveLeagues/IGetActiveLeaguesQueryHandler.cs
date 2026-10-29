namespace ProjectFootballSim.Leagues.Contracts.CatalogFeatures.GetActiveLeagues;

public interface IGetActiveLeaguesQueryHandler
{
    Task<IReadOnlyDictionary<int, ActiveLeagueDto>> HandleAsync(CancellationToken cancellationToken);
}
