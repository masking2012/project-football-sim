using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.Common.Models;
using ProjectFootballSim.Leagues.Application.Common.Services;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GameFeatures.CreateGameLeague;

public sealed class CreateGameLeagueCommandHandler(
    ILeaguesCatalog leaguesCatalog,
    LeaguesDbContext dbContext,
    LeagueFixtureGenerator leagueFixtureGenerator)
{
    public async Task HandleAsync(CreateGameLeagueCommand command, CancellationToken cancellationToken)
    {
        LeagueDto? league = await leaguesCatalog.GetByIdAsync(command.LeagueId, cancellationToken).ConfigureAwait(false);
        if (league is null)
            throw new InvalidOperationException($"League with ID '{command.LeagueId}' not found.");

        //TODO: add logic for second and subsequent seasons

        List<int> teamIds = await dbContext.LeagueTeams
            .Where(lt => lt.LeagueId == command.LeagueId)
            .Select(lt => lt.TeamId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (teamIds.Count != league.TeamsCount)
            throw new InvalidOperationException($"League with ID '{command.LeagueId}' has {league.TeamsCount} teams, but {teamIds.Count} teams were found in the database.");

        var gameLeague = new GameLeague
        (
            gameId: command.GameId,
            leagueId: command.LeagueId,
            userId: command.UserId,
            seasonId: command.SeasonId
        );
        foreach (int teamId in teamIds)
        {
            gameLeague.AddGameLeagueTeam(new GameLeagueTeam
            (
                gameLeagueId: gameLeague.Id,
                teamId: teamId
            ));
        }

        var leagueMatches = await leagueFixtureGenerator
            .GenerateAsync(command.LeagueId, gameLeague.Id, teamIds, command.SeasonStartDate, cancellationToken)
            .ConfigureAwait(false);
        foreach (GameLeagueMatch match in leagueMatches)
        {
            gameLeague.AddGameLeagueMatch(match);
        }

        dbContext.GameLeagues.Add(gameLeague);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
