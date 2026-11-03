using ProjectFootballSim.Calendar.Contracts.Features.ScheduleGameFixtures;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.Events;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application.EventListeners;

internal sealed class GameLeagueRoundsReadyToScheduleEventListener(
    IScheduleGameFixturesCommandHandler scheduleGameFixturesCommandHandler) : IEventListener<GameLeagueRoundsReadyToScheduleEvent>
{
    public async Task HandleAsync(GameLeagueRoundsReadyToScheduleEvent @event, CancellationToken cancellationToken)
    {
        var provisioningRounds = @event.Rounds
            .Select(round => new ScheduleGameFixturesRoundItem(
                GameCompetitionId: round.GameLeagueId,
                CompetitionType: CompetitionType.League,
                RoundId: round.Round,
                BaseDate: round.BaseDate,
                AvailableDaysOfWeek: round.MatchDays))
            .ToList();

        var command = new ScheduleGameFixturesCommand(
            UserId: @event.UserId,
            GameId: @event.GameId,
            Rounds: provisioningRounds);

        await scheduleGameFixturesCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
    }
}
