using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.SimulateLeagueGameDay;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Features.RegularTime;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.SimulateLeagueGameDay;

internal sealed class SimulateLeagueGameDayCommandHandler(
    ISimulateLeagueDayRepository repository,
    IGetTeamsByIdsQueryHandler getTeamsByIdsQueryHandler,
    SimulateRegularTimeCommandHandler simulateRegularTimeCommandHandler)
    : ISimulateLeagueGameDayCommandHandler
{
    public async Task HandleAsync(SimulateLeagueGameDayCommand command, CancellationToken cancellationToken)
    {
        var startDate = new DateTime(command.Date.Year, command.Date.Month, command.Date.Day);
        var endDate = startDate.AddDays(1);

        IReadOnlyList<GameLeague> gameLeaguesWithMatches = await repository
            .GetLeaguesWithMatchesForDateAsync(command.UserId, command.GameId, startDate, endDate, cancellationToken)
            .ConfigureAwait(false);

        if (gameLeaguesWithMatches.Count == 0)
            return;

        var teams = await GetTeamsAsync(gameLeaguesWithMatches, cancellationToken).ConfigureAwait(false);

        foreach (GameLeague gameLeague in gameLeaguesWithMatches)
        {
            foreach(GameLeagueMatch match in gameLeague.GameLeagueMatches.Where(match => !match.HomeTeamScore.HasValue && !match.AwayTeamScore.HasValue))
                SimulateMatch(match, gameLeague, teams);
        }

        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private Task<IReadOnlyDictionary<int, TeamByIdsDto>> GetTeamsAsync(
        IReadOnlyList<GameLeague> gameLeaguesWithMatches,
        CancellationToken cancellationToken)
    {
        List<int> teamIds = gameLeaguesWithMatches
            .SelectMany(gl => gl.GameLeagueMatches)
            .SelectMany(match => new[] { match.HomeTeamId, match.AwayTeamId })
            .Distinct()
            .ToList();
        return getTeamsByIdsQueryHandler.HandleAsync(teamIds, cancellationToken);
    }

    private void SimulateMatch(
        GameLeagueMatch match,
        GameLeague gameLeague,
        IReadOnlyDictionary<int, TeamByIdsDto> teams)
    {
        var home = CreateMatchTeam(match.HomeTeamId, teams);
        var away = CreateMatchTeam(match.AwayTeamId, teams);
        var score = simulateRegularTimeCommandHandler.Handle(new SimulateRegularTimeCommand(
            home,
            away,
            new MatchSettingsDto(HasHomeAdvantage: true)));

        gameLeague.RecordMatchResult(match.Id, score.HomeScore, score.AwayScore);
    }

    private static MatchTeamDto CreateMatchTeam(int teamId, IReadOnlyDictionary<int, TeamByIdsDto> teams)
    {
        if (!teams.TryGetValue(teamId, out TeamByIdsDto? team))
            throw new InvalidOperationException($"Team with Id {teamId} not found.");

        return new MatchTeamDto(Id: teamId, Attack: team.Attack, Midfield: team.Midfield, Defence: team.Defence);
    }
}
