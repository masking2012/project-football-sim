using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.CreateGameLeague;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.Events;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.CreateGameLeague;

internal sealed class CreateGameLeagueCommandHandler(
    ICachedLeaguesRetriever leaguesRetriever,
    ILeaguesTeamsRepository leaguesTeamsRepository,
    IGameLeagueWriteRepository writeRepository,
    LeagueFixtureGenerator leagueFixtureGenerator,
    IEventPublisher eventPublisher) : ICreateGameLeagueCommandHandler
{
    public async Task HandleAsync(CreateGameLeagueCommand command, CancellationToken cancellationToken)
    {
        LeagueDto? league = await leaguesRetriever.GetLeagueAsync(command.LeagueVersionId, cancellationToken).ConfigureAwait(false);
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
        List<EventLeagueMatch> eventLeagueMatches = [];

        foreach (GameLeagueMatch match in leagueMatches)
        {
            gameLeague.AddGameLeagueMatch(match);

            eventLeagueMatches.Add(new EventLeagueMatch(
                MatchId: match.Id,
                HomeTeamId: match.HomeTeamId,
                AwayTeamId: match.AwayTeamId,
                ScheduledAt: match.Date,
                Round: match.Round
            ));
        }
            
        await writeRepository.AddAsync(gameLeague, cancellationToken).ConfigureAwait(false);
        await writeRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await eventPublisher.PublishAsync(new GameLeagueMatchesProvisionalEvent(
            UserId: command.UserId,
            GameId: command.GameId,
            LeagueVersionId: league.LeagueVersionId,
            GameLeagueId: gameLeague.Id,
            Matches: eventLeagueMatches), cancellationToken).ConfigureAwait(false);
    }
}
