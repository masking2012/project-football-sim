using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Features.RegularTime;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByIds;
using ProjectFootballSim.Leagues.Application.GameState.Services;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.SimulateLeagueGameDay;

public sealed class SimulateLeagueGameDayCommandHandler(
    IGameLeagueRepository readRepository,
    IGameLeagueWriteRepository writeRepository,
    GetTeamsByIdsQueryHandler getTeamsByIdsQueryHandler,
    SimulateRegularTimeCommandHandler simulateRegularTimeCommandHandler)
{
    public async Task HandleAsync(SimulateLeagueGameDayCommand command, CancellationToken cancellationToken)
    {
        var startDate = new DateTime(command.Date.Year, command.Date.Month, command.Date.Day);
        var endDate = startDate.AddDays(1);
        var todaysMatches = await GetTodaysMatchesAsync(command, startDate, endDate, cancellationToken).ConfigureAwait(false);
        var teams = await GetTeamsAsync(todaysMatches, cancellationToken).ConfigureAwait(false);

        foreach (var matchesForLeague in todaysMatches.GroupBy(x => x.GameLeagueId))
        {
            var gameLeague = await GetGameLeagueAsync(command.UserId, matchesForLeague.Key, cancellationToken).ConfigureAwait(false);
            foreach (var match in matchesForLeague)
                SimulateMatch(match, gameLeague, teams);
        }

        await writeRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<GameLeagueMatch>> GetTodaysMatchesAsync(
        SimulateLeagueGameDayCommand command,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        return await readRepository
            .GetMatchesForDateAsync(command.UserId, command.GameId, startDate, endDate, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<IReadOnlyDictionary<int, TeamDto>> GetTeamsAsync(
        IReadOnlyCollection<GameLeagueMatch> matches,
        CancellationToken cancellationToken)
    {
        var teamIds = matches
            .SelectMany(match => new[] { match.HomeTeamId, match.AwayTeamId })
            .Distinct()
            .ToList();

        return await getTeamsByIdsQueryHandler.HandleAsync(teamIds, cancellationToken).ConfigureAwait(false);
    }

    private async Task<GameLeague> GetGameLeagueAsync(
        Guid userId,
        Guid gameLeagueId,
        CancellationToken cancellationToken)
    {
        var gameLeague = await readRepository
            .GetAsync(userId, gameLeagueId, cancellationToken)
            .ConfigureAwait(false);

        return gameLeague ?? throw new InvalidOperationException(
            $"GameLeague with Id {gameLeagueId} not found for UserId {userId}");
    }

    private void SimulateMatch(
        GameLeagueMatch match,
        GameLeague gameLeague,
        IReadOnlyDictionary<int, TeamDto> teams)
    {
        var home = CreateMatchTeam(match.HomeTeamId, teams);
        var away = CreateMatchTeam(match.AwayTeamId, teams);
        var score = simulateRegularTimeCommandHandler.Handle(new SimulateRegularTimeCommand(
            home,
            away,
            new MatchSettingsDto(HasHomeAdvantage: true)));

        gameLeague.RecordMatchResult(match.Id, score.HomeScore, score.AwayScore);
    }

    private static MatchTeamDto CreateMatchTeam(int teamId, IReadOnlyDictionary<int, TeamDto> teams)
    {
        if (!teams.TryGetValue(teamId, out TeamDto? team))
            throw new InvalidOperationException($"Team with Id {teamId} not found.");

        return new MatchTeamDto(Id: teamId, Attack: team.Attack, Midfield: team.Midfield, Defence: team.Defence);
    }

}
