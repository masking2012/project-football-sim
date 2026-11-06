using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.GameState.Interfaces;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByIds;
using ProjectFootballSim.Locations.Contracts.Features.GetCountries;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByIds;

internal sealed class GetGameLeagueMatchesByIdsQueryHandler(
    IGetGameLeagueMatchesByIdsRepository repository,
    IGameLeaguesService gameLeagueService,
    IGetTeamsByIdsQueryHandler getTeamsByIdsQueryHandler,
    IGetCountriesQueryHandler getCountriesQueryHandler) : IGetGameLeagueMatchesByIdsQueryHandler
{
    public async Task<IReadOnlyList<GetGameLeagueMatchDto>> HandleAsync(
        GetGameLeagueMatchesByIdsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Ids.Count == 0)
            return [];

        IReadOnlyList<LeagueMatchDto> matches = await repository
            .GetAsync(userId: query.UserId, gameId: query.GameId, ids: query.Ids, cancellationToken)
            .ConfigureAwait(false);

        var missingIds = query.Ids.Except(matches.Select(m => m.Id)).ToList();
        if (missingIds.Count > 0)
            throw new KeyNotFoundException($"League matches were not found: {string.Join(", ", missingIds)}.");

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
