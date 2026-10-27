using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;
using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.CreateGameLeague;

public sealed class CreateGameLeagueCommandHandler(
    ILeaguesCatalog leaguesCatalog,
    ILeaguesTeamsRepository leaguesTeamsRepository,
    IGameLeagueWriteRepository writeRepository,
    LeagueFixtureGenerator leagueFixtureGenerator)
{
    public async Task HandleAsync(CreateGameLeagueCommand command, CancellationToken cancellationToken)
    {
        LeagueDto? league = await leaguesCatalog.GetByLeagueVersionIdAsync(command.LeagueVersionId, cancellationToken).ConfigureAwait(false);
        if (league is null)
            throw new InvalidOperationException($"League with ID '{command.LeagueVersionId}' not found.");

        //TODO: add logic for second and subsequent seasons

        IReadOnlyList<int> teamIds = await leaguesTeamsRepository
            .GetTeamIdsAsync(league.LeagueVersionId, cancellationToken)
            .ConfigureAwait(false);

        if (teamIds.Count != league.TeamsCount)
            throw new InvalidOperationException($"League with ID '{command.LeagueVersionId}' has {league.TeamsCount} teams, but {teamIds.Count} teams were found in the database.");

        var gameLeague = new GameLeague
        (
            gameId: command.GameId,
            leagueVersionId: league.LeagueVersionId,
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
            .GenerateAsync(command.LeagueVersionId, gameLeague.Id, teamIds, command.SeasonStartDate, cancellationToken)
            .ConfigureAwait(false);
        foreach (GameLeagueMatch match in leagueMatches)
        {
            gameLeague.AddGameLeagueMatch(match);
        }

        await writeRepository.AddAsync(gameLeague, cancellationToken).ConfigureAwait(false);
        await writeRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
