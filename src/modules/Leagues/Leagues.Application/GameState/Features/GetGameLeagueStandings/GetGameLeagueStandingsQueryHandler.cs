using ProjectFootballSim.Leagues.Application.GameState.Interfaces;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueStandings;

public sealed class GetGameLeagueStandingsQueryHandler(IGameLeaguesRepository repository)
{
    public async Task<IReadOnlyList<GameLeagueTeamDto>> HandleAsync(
        GetGameLeagueStandingsQuery query,
        CancellationToken cancellationToken)
    {
        GameLeague? league = await repository
            .GetAsync(query.UserId, query.GameId, query.SeasonId, query.LeagueVersionId, cancellationToken)
            .ConfigureAwait(false);

        if (league is null)
            throw new InvalidOperationException($"Game league not found for user {query.UserId}, game {query.GameId}, league version {query.LeagueVersionId}");

        return league.GameLeagueTeams
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalsFor - t.GoalsAgainst)
            .ThenByDescending(t => t.GoalsFor)
            .ThenBy(t => t.TeamId)
            .Select((t, index) => new GameLeagueTeamDto(
                Position: index + 1,
                TeamId: t.TeamId,
                Wins: t.Wins,
                Draws: t.Draws,
                Losses: t.Losses,
                GoalsFor: t.GoalsFor,
                GoalsAgainst: t.GoalsAgainst,
                Points: t.Points))
            .ToList();
    }
}
