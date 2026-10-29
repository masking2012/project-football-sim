namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueStandings;

public interface IGetGameLeagueStandingsQueryHandler
{
    Task<IReadOnlyList<LeagueStandingsTeamDto>> HandleAsync(GetGameLeagueStandingsQuery query, CancellationToken cancellationToken);
}
