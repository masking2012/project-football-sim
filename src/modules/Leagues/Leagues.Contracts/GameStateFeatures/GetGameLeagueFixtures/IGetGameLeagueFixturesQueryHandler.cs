namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueFixtures;

public interface IGetGameLeagueFixturesQueryHandler
{
    Task<IReadOnlyList<LeagueMatchDto>> HandleAsync(GetGameLeagueFixturesQuery query, CancellationToken cancellationToken);
}
