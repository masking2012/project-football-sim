using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Features.RegularTime;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByIds;

namespace ProjectFootballSim.Leagues.Application.GameFeatures.SimulateLeagueGameDay;

public sealed class SimulateLeagueGameDayCommandHandler(
    LeaguesDbContext dbContext,
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

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<List<GameLeagueMatch>> GetTodaysMatchesAsync(
        SimulateLeagueGameDayCommand command,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        return await dbContext.GameLeagueMatches
            .Include(x => x.GameLeague)
            .Where(x =>
                x.GameLeague.UserId == command.UserId
                && x.GameLeague.GameId == command.GameId
                && x.Date >= startDate
                && x.Date < endDate)
            .ToListAsync(cancellationToken)
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
        var gameLeague = await dbContext.GameLeagues
            .Include(x => x.GameLeagueTeams)
            .SingleOrDefaultAsync(x => x.UserId == userId && x.Id == gameLeagueId, cancellationToken)
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

        match.SetScore(score.HomeScore, score.AwayScore);
        UpdateStandings(gameLeague, match, score);
    }

    private static MatchTeamDto CreateMatchTeam(int teamId, IReadOnlyDictionary<int, TeamDto> teams)
    {
        if (!teams.TryGetValue(teamId, out TeamDto? team))
            throw new InvalidOperationException($"Team with Id {teamId} not found.");

        return new MatchTeamDto(teamId, team.Attack, team.Midfield, team.Defence);
    }

    private static void UpdateStandings(GameLeague gameLeague, GameLeagueMatch match, ScoreResultDto score)
    {
        var homeTeam = gameLeague.GameLeagueTeams.Single(x => x.TeamId == match.HomeTeamId);
        var awayTeam = gameLeague.GameLeagueTeams.Single(x => x.TeamId == match.AwayTeamId);

        if (score.HomeScore > score.AwayScore)
        {
            homeTeam.AddWin(score.HomeScore, score.AwayScore);
            awayTeam.AddLoss(score.AwayScore, score.HomeScore);
        }
        else if (score.HomeScore < score.AwayScore)
        {
            homeTeam.AddLoss(score.HomeScore, score.AwayScore);
            awayTeam.AddWin(score.AwayScore, score.HomeScore);
        }
        else
        {
            homeTeam.AddDraw(score.HomeScore, score.AwayScore);
            awayTeam.AddDraw(score.AwayScore, score.HomeScore);
        }
    }
}
