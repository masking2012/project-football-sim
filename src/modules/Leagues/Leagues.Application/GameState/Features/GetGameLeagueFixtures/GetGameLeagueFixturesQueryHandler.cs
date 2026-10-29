using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueFixtures;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueFixtures;

public sealed class GetGameLeagueFixturesQueryHandler(
    IGetGameLeagueMatchesRepository repository,
    IGetTeamsByIdsQueryHandler getTeamsByIdsQueryHandler) : IGetGameLeagueFixturesQueryHandler
{
    public async Task<IReadOnlyList<LeagueMatchDto>> HandleAsync(
        GetGameLeagueFixturesQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<GameLeagueMatchDto> gameLeagueMatches = await repository
            .GetAsync(query.UserId, query.GameId, query.SeasonId, query.LeagueVersionId, cancellationToken)
            .ConfigureAwait(false);

        var teamsIds = gameLeagueMatches.SelectMany(t => new[] { t.HomeTeamId, t.AwayTeamId }).Distinct().ToList();
        IReadOnlyDictionary<int, TeamByIdsDto> teams = await getTeamsByIdsQueryHandler.HandleAsync(teamsIds, cancellationToken).ConfigureAwait(false);

        return gameLeagueMatches
            .Select(f => new LeagueMatchDto(
                Id: f.Id,
                Date: f.Date,
                HomeTeamId: f.HomeTeamId,
                AwayTeamId: f.AwayTeamId,
                HomeTeamName: teams[f.HomeTeamId].Name,
                AwayTeamName: teams[f.AwayTeamId].Name,
                HomeTeamScore: f.HomeTeamGoals,
                AwayTeamScore: f.AwayTeamGoals,
                Round: f.Round))
            .ToList();
    }
}
