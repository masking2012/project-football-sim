using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Application.Persistence.Repositories;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;

public sealed class GetGameLeagueMatchesByDateQueryHandler(
    IGameLeagueRepository repository,
    IGameLeagueCatalog gameLeagueCatalog)
{
    public async Task<IReadOnlyList<GameLeagueMatchDto>> HandleAsync(
        GetGameLeagueMatchesByDateQuery query,
        CancellationToken cancellationToken)
    {
        var leagues = (await gameLeagueCatalog
            .GetByGameAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false))
            .ToDictionary(league => league.LeagueVersionId);

        var startDate = new DateTime(query.Date.Year, query.Date.Month, query.Date.Day);
        var endDate = startDate.AddDays(1);

        var matches = await repository
            .GetMatchesByDateAsync(query.UserId, query.GameId, startDate, endDate, cancellationToken)
            .ConfigureAwait(false);

        return matches
            .Select(x => new GameLeagueMatchDto(
                Id: x.Match.Id,
                HomeTeamId: x.Match.HomeTeamId,
                AwayTeamId: x.Match.AwayTeamId,
                HomeTeamScore: x.Match.HomeTeamScore,
                AwayTeamScore: x.Match.AwayTeamScore,
                Round: x.Match.Round,
                LeagueName: leagues[x.LeagueVersionId].Name,
                LeagueId: leagues[x.LeagueVersionId].Id,
                CountryId: leagues[x.LeagueVersionId].CountryId,
                Order: leagues[x.LeagueVersionId].Order))
            .ToList();
    }
}
