namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueStandings;

public interface IGetGameLeagueStandingsRepository
{
    Task<IReadOnlyList<GameLeagueTeamDto>> GetAsync(
        Guid userId,
        Guid gameId,
        Guid seasonId,
        int leagueVersionId,
        CancellationToken cancellationToken);
}
