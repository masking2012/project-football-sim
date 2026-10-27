namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

public interface ILeaguesTeamsRepository
{
    Task<IReadOnlyList<int>> GetTeamIdsAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);
}
