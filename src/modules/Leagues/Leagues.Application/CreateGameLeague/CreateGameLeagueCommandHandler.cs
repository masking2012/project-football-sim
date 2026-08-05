using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.CreateGameLeague;

public sealed class CreateGameLeagueCommandHandler(LeaguesDbContext dbContext)
{
    public async Task HandleAsync(CreateGameLeagueCommand command, CancellationToken cancellationToken)
    {
        var league = await dbContext.Leagues
            .FirstOrDefaultAsync(l => l.Id == command.LeagueId, cancellationToken)
            .ConfigureAwait(false);
        if (league is null)
            throw new InvalidOperationException($"League with ID '{command.LeagueId}' not found.");

        List<int> teamIds = await dbContext.LeagueTeams
            .Where(lt => lt.LeagueId == command.LeagueId)
            .Select(lt => lt.TeamId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
            
        var gameLeague = new GameLeague
        (
            gameId: command.GameId,
            leagueId: command.LeagueId,
            userId: command.UserId
        );
        foreach ( var teamId in teamIds)
        {
            gameLeague.AddGameLeagueTeam(new GameLeagueTeam
            (
                gameLeagueId: gameLeague.Id,
                teamId: teamId
            ));
        }

        dbContext.GameLeagues.Add(gameLeague);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
