namespace ProjectFootballSim.Leagues.Application.GameState.Features.CheckGameLeagueMatchesByDate;

public interface ICheckGameLeagueMatchesRepository
{
    Task<bool> MatchesExist(
        Guid userId,
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken);
}
