namespace ProjectFootballSim.Leagues.Application.Catalog.Interfaces;

public interface ILeaguesTeamsRepository
{
    Task<IReadOnlyList<int>> GetTeamIdsAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);
}
