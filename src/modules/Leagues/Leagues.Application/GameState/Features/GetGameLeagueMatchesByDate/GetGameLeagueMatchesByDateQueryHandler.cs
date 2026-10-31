using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.GameState.Interfaces;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByDate;
using ProjectFootballSim.Locations.Contracts.Features.GetCountries;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;

internal sealed class GetGameLeagueMatchesByDateQueryHandler(
    IGetGameLeagueMatchesByDateRepository repository,
    IGameLeaguesService gameLeagueService,
    IGetTeamsByIdsQueryHandler getTeamsByIdsQueryHandler,
    IGetCountriesQueryHandler getCountriesQueryHandler) : IGetGameLeagueMatchesByDateQueryHandler
{
    public async Task<IReadOnlyList<GetGameLeagueMatchDto>> HandleAsync(
        GetGameLeagueMatchesByDateQuery query,
        CancellationToken cancellationToken)
    {
        var startDate = new DateTime(query.Date.Year, query.Date.Month, query.Date.Day);
        var endDate = startDate.AddDays(1);

        IReadOnlyList<LeagueMatchDto> matches = await repository
            .GetAsync(userId: query.UserId, gameId: query.GameId, startDate, endDate, cancellationToken)
            .ConfigureAwait(false);
        if (matches is null || matches.Count == 0)
            return [];

        Dictionary<int, LeagueDto> gameLeagues = (await gameLeagueService
            .GetGameLeaguesAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false))
            .ToDictionary(league => league.LeagueVersionId);

        List<int> teamsIds = matches.SelectMany(m => new[] { m.HomeTeamId, m.AwayTeamId }).Distinct().ToList();
        IReadOnlyDictionary<int, TeamByIdsDto> teams = await getTeamsByIdsQueryHandler
            .HandleAsync(teamsIds, cancellationToken)
            .ConfigureAwait(false);

        Dictionary<int, GetCountryDto> countries = (await getCountriesQueryHandler.HandleAsync(cancellationToken).ConfigureAwait(false))
            .ToDictionary(c => c.Id);

        return matches
            .Select(x => new GetGameLeagueMatchDto(
                Id: x.Id,
                HomeTeamId: x.HomeTeamId,
                AwayTeamId: x.AwayTeamId,
                HomeTeamName: teams[x.HomeTeamId].Name,
                AwayTeamName: teams[x.AwayTeamId].Name,
                HomeTeamScore: x.HomeTeamScore,
                AwayTeamScore: x.AwayTeamScore,
                LeagueVersionId: x.LeagueVersionId,
                LeagueName: gameLeagues[x.LeagueVersionId].Name,
                LeagueRound: x.LeagueRound,
                LeagueOrder: gameLeagues[x.LeagueVersionId].Order,
                CountryId: gameLeagues[x.LeagueVersionId].CountryId,
                CountryName: countries[gameLeagues[x.LeagueVersionId].CountryId].Name))
            .ToList();
    }
}
