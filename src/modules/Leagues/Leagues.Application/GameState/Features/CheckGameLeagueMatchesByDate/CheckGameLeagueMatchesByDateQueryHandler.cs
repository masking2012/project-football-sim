using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.CheckGameLeagueMatchesByDate;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.CheckGameLeagueMatchesByDate;

internal sealed class CheckGameLeagueMatchesByDateQueryHandler
    (ICheckGameLeagueMatchesRepository repository) : ICheckGameLeagueMatchesByDateQueryHandler
{
    public async Task<CheckGameLeagueMatchesByDateQueryResult> HandleAsync(CheckGameLeagueMatchesByDateQuery query, CancellationToken cancellationToken)
    {
        var startDate = new DateTime(query.Date.Year, query.Date.Month, query.Date.Day);
        var endDate = startDate.AddDays(1);

        bool matchesExist = await repository.MatchesExist(
            userId: query.UserId,
            gameId: query.GameId,
            startDate: startDate,
            endDate: endDate,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return new CheckGameLeagueMatchesByDateQueryResult(matchesExist);
    }
}
