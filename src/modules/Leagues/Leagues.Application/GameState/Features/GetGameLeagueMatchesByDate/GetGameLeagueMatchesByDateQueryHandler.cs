using ProjectFootballSim.Leagues.Application.GameState.Interfaces;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;

public sealed class GetGameLeagueMatchesByDateQueryHandler(
    IGameLeaguesRepository repository,
    IGameLeaguesRetriever gameLeagueCatalog)
{
    public async Task<IReadOnlyList<GameLeagueMatchDto>> HandleAsync(
        GetGameLeagueMatchesByDateQuery query,
        CancellationToken cancellationToken)
    {
        var leagues = (await gameLeagueCatalog
            .GetLeaguesAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false))
            .ToDictionary(league => league.LeagueVersionId);

        var startDate = new DateTime(query.Date.Year, query.Date.Month, query.Date.Day);
        var endDate = startDate.AddDays(1);

        IReadOnlyList<GameLeagueMatch> matches = await repository
            .GetMatchesForDateAsync(query.UserId, query.GameId, startDate, endDate, cancellationToken)
            .ConfigureAwait(false);

        var leagueVersionIdsByGameLeague = await repository
            .GetLeagueVersionIdsByGameLeagueAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false);

        return matches
            .Select(x => new GameLeagueMatchDto(
                Id: x.Id,
                HomeTeamId: x.HomeTeamId,
                AwayTeamId: x.AwayTeamId,
                HomeTeamScore: x.HomeTeamScore,
                AwayTeamScore: x.AwayTeamScore,
                Round: x.Round,
                LeagueName: leagues[leagueVersionIdsByGameLeague[x.GameLeagueId]].Name,
                LeagueId: leagues[leagueVersionIdsByGameLeague[x.GameLeagueId]].Id,
                CountryId: leagues[leagueVersionIdsByGameLeague[x.GameLeagueId]].CountryId,
                Order: leagues[leagueVersionIdsByGameLeague[x.GameLeagueId]].Order))
            .ToList();
    }
}
