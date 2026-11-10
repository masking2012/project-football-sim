using ProjectFootballSim.Calendar.Contracts.Features.ScheduleGameFixtures;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.UpdateGameLeagueMatches;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.UpdateGameLeagueRoundStatus;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Leagues.Application.GameState.EventListeners;

internal sealed class ScheduleGameFixturesEventListener(
    IUpdateGameLeagueMatchesCommandHandler updateGameLeagueMatchesCommandHandler,
    IUpdateGameLeagueRoundStatusCommandHandler updateGameLeagueRoundStatusCommandHandler)
    : IEventListener<ScheduleGameFixturesEvent>
{
    public async Task HandleAsync(ScheduleGameFixturesEvent @event, CancellationToken cancellationToken)
    {
        var leagueVersions = @event.Versions
            .Where(version => version.CompetitionType == CompetitionType.League)
            .ToList();

        if (leagueVersions.Count == 0)
            return;

        await updateGameLeagueMatchesCommandHandler.HandleAsync(new UpdateGameLeagueMatchesCommand(
                UserId: @event.UserId,
                GameId: @event.GameId,
                Matches: leagueVersions
                    .SelectMany(version => version.Matches)
                    .Select(match => new GameLeagueMatchDateReference(match.MatchId, match.Date))
                    .ToList()),
            cancellationToken).ConfigureAwait(false);

        await updateGameLeagueRoundStatusCommandHandler.HandleAsync(new UpdateGameLeagueRoundStatusCommand(
                UserId: @event.UserId,
                GameId: @event.GameId,
                Rounds: leagueVersions
                    .Select(version => new GameLeagueRoundReference(version.GameCompetitionId, version.Round))
                    .ToList(),
                Status: GameLeagueRoundStatusContract.Scheduled),
            cancellationToken).ConfigureAwait(false);
    }
}
