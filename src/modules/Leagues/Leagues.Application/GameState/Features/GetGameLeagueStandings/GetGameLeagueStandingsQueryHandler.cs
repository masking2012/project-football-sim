using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueStandings;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueStandings;

internal sealed class GetGameLeagueStandingsQueryHandler(
    IGetGameLeagueStandingsRepository repository,
    IGetTeamsByIdsQueryHandler getTeamsByIdsQueryHandler) : IGetGameLeagueStandingsQueryHandler
{
    public async Task<IReadOnlyList<LeagueStandingsTeamDto>> HandleAsync(
        GetGameLeagueStandingsQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<GameLeagueTeamDto> gameLeagueStandindsTeams = await repository
            .GetAsync(query.UserId, query.GameId, query.SeasonId, query.LeagueVersionId, cancellationToken)
            .ConfigureAwait(false);

        var gameLeagueStandindsTeamsIds = gameLeagueStandindsTeams.Select(t => t.TeamId).ToList();
        IReadOnlyDictionary<int, TeamByIdsDto> teams = await getTeamsByIdsQueryHandler.HandleAsync(gameLeagueStandindsTeamsIds, cancellationToken).ConfigureAwait(false);

        return gameLeagueStandindsTeams
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalsFor - t.GoalsAgainst)
            .ThenByDescending(t => t.GoalsFor)
            .ThenBy(t => teams[t.TeamId].Name)
            .Select((t, index) => new LeagueStandingsTeamDto(
                Position: index + 1,
                TeamId: t.TeamId,
                Name: teams[t.TeamId].Name,
                Wins: t.Wins,
                Draws: t.Draws,
                Losses: t.Losses,
                GoalsFor: t.GoalsFor,
                GoalsAgainst: t.GoalsAgainst,
                Points: t.Points))
            .ToList();
    }
}
