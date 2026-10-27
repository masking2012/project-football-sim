using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueFixtures;

public sealed class GetGameLeagueFixturesQueryHandler(IGameLeagueRepository repository)
{
    public async Task<IReadOnlyList<GameLeagueMatchDto>> HandleAsync(
        GetGameLeagueFixturesQuery query,
        CancellationToken cancellationToken)
    {
        GameLeague? league = await repository
            .GetAsync(query.UserId, query.GameId, query.SeasonId, query.LeagueVersionId, cancellationToken)
            .ConfigureAwait(false);

        if (league is null)
            throw new InvalidOperationException("League not found");

        return league.GameLeagueMatches
            .Select(f => new GameLeagueMatchDto(
                Id: f.Id,
                Date: f.Date,
                HomeTeamId: f.HomeTeamId,
                AwayTeamId: f.AwayTeamId,
                HomeTeamScore: f.HomeTeamScore,
                AwayTeamScore: f.AwayTeamScore,
                Round: f.Round))
            .ToList();
    }
}
