using ProjectFootballSim.Calendar.Contracts.Features.ProvisionGameFixtures;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.Events;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application.EventListeners;

internal sealed class GameLeagueMatchesProvisionalEventListener(
    IProvisionGameFixturesCommandHandler provisionGameFixturesCommandHandler) : IEventListener<GameLeagueMatchesProvisionalEvent>
{
    public async Task HandleAsync(GameLeagueMatchesProvisionalEvent @event, CancellationToken cancellationToken)
    {
        var fixtures = @event.Matches
            .Select(match => new ProvisionGameFixtureItem(
                MatchId: match.MatchId,
                HomeTeamId: match.HomeTeamId,
                AwayTeamId: match.AwayTeamId,
                ScheduledAt: match.ScheduledAt,
                RoundId: match.Round))
            .ToList();

        var command = new ProvisionGameFixturesCommand(
            UserId: @event.UserId,
            GameId: @event.GameId,
            GameLeagueId: @event.GameLeagueId,
            LeagueVersionId: @event.LeagueVersionId,
            Fixtures: fixtures);

        await provisionGameFixturesCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
    }
}
