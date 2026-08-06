using Microsoft.EntityFrameworkCore;
using ProjectFootball.Core.Simulation.ChampionshipSimulation;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;
using ProjectFootballSim.Seasons.Application.Features.GetPlayerSeasons;

namespace ProjectFootballSim.Leagues.Application.GameFeatures.CreateGameLeague;

public sealed class CreateGameLeagueCommandHandler(
    LeaguesDbContext dbContext,
    GetGameSeasonsQueryHandler getGameSeasonsQueryHandler,
    LeagueFixtureGenerator leagueFixtureGenerator)
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

        DateTime seasonStartDate = await GetSeasonStartDateAsync(command, cancellationToken).ConfigureAwait(false);

        var leagueMatches = leagueFixtureGenerator.Generate(command.LeagueId, gameLeague.Id, teamIds, seasonStartDate);
        foreach (GameLeagueMatch match in leagueMatches)
        {
            gameLeague.AddGameLeagueMatch(match);
        }

        dbContext.GameLeagues.Add(gameLeague);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<DateTime> GetSeasonStartDateAsync(CreateGameLeagueCommand command, CancellationToken cancellationToken)
    {
        var query = new GetGameSeasonsQuery(GameId: command.GameId, UserId: command.UserId);
        var seasons = await getGameSeasonsQueryHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
        var season = seasons.SingleOrDefault(s => s.Id == command.SeasonId);
        if (season is null)
            throw new InvalidOperationException($"Season with ID '{command.SeasonId}' not found.");
        return season.StartDate;
    }
}
