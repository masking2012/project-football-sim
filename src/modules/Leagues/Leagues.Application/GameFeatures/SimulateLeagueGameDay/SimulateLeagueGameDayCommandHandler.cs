using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Features.RegularTime;
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

        var todaysMatches = await dbContext.GameLeagueMatches
            .Include(x => x.GameLeague)
            .Where(x =>
                x.GameLeague.UserId == command.UserId
                && x.GameLeague.GameId == command.GameId
                && x.Date >= startDate && x.Date < endDate)           
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var allTeamsIds = todaysMatches.SelectMany(x => new [] { x.HomeTeamId, x.AwayTeamId }).Distinct().ToList();
        var teams = await getTeamsByIdsQueryHandler.HandleAsync(allTeamsIds, cancellationToken).ConfigureAwait(false);

        foreach (var group in todaysMatches.GroupBy(x => x.GameLeagueId))
        {
            var gameLeagueId = group.Key;
            GameLeague? gameLeague = await dbContext.GameLeagues
                .Include(x => x.GameLeagueTeams)
                .SingleOrDefaultAsync(x =>
                    x.UserId == command.UserId
                    && x.Id == gameLeagueId, cancellationToken)
                .ConfigureAwait(false);
            if (gameLeague is null)
                throw new InvalidOperationException($"GameLeague with Id {gameLeagueId} not found for UserId {command.UserId}");

            var matches = group.ToList();

            foreach (var match in matches)
            {
                var home = new MatchTeamDto
                (
                    Id: match.HomeTeamId,
                    Attack: teams[match.HomeTeamId].Attack,
                    Midfield: teams[match.HomeTeamId].Midfield,
                    Defence: teams[match.HomeTeamId].Defence
                );
                var away = new MatchTeamDto
                (
                    Id: match.AwayTeamId,
                    Attack: teams[match.AwayTeamId].Attack,
                    Midfield: teams[match.AwayTeamId].Midfield,
                    Defence: teams[match.AwayTeamId].Defence
                );

                ScoreResultDto scoreResultDto = simulateRegularTimeCommandHandler.Handle(new SimulateRegularTimeCommand(
                    home, away, new MatchSettingsDto(HasHomeAdvantage: true)));
                match.SetScore(scoreResultDto.HomeScore, scoreResultDto.AwayScore);

                if (scoreResultDto.HomeScore > scoreResultDto.AwayScore)
                {
                    gameLeague.GameLeagueTeams.Single(x => x.TeamId == match.HomeTeamId).AddWin(scoreResultDto.HomeScore, scoreResultDto.AwayScore);
                    gameLeague.GameLeagueTeams.Single(x => x.TeamId == match.AwayTeamId).AddLoss(scoreResultDto.AwayScore, scoreResultDto.HomeScore);
                }
                else if (scoreResultDto.HomeScore < scoreResultDto.AwayScore)
                {
                    gameLeague.GameLeagueTeams.Single(x => x.TeamId == match.HomeTeamId).AddLoss(scoreResultDto.HomeScore, scoreResultDto.AwayScore);
                    gameLeague.GameLeagueTeams.Single(x => x.TeamId == match.AwayTeamId).AddWin(scoreResultDto.AwayScore, scoreResultDto.HomeScore);
                }
                else
                {
                    gameLeague.GameLeagueTeams.Single(x => x.TeamId == match.HomeTeamId).AddDraw(scoreResultDto.HomeScore, scoreResultDto.AwayScore);
                    gameLeague.GameLeagueTeams.Single(x => x.TeamId == match.AwayTeamId).AddDraw(scoreResultDto.AwayScore, scoreResultDto.HomeScore);
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
