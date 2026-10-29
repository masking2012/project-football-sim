namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueFixtures;

public interface IGetGameLeagueMatchesRepository
{
    Task<IReadOnlyList<GameLeagueMatchDto>> GetAsync(
        Guid userId,
        Guid gameId,
        Guid seasonId,
        int leagueVersionId,
        CancellationToken cancellationToken);
}
