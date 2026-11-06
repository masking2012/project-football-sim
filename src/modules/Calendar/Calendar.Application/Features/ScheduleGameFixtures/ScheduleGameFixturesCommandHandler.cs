using ProjectFootballSim.Calendar.Contracts.Features.ScheduleGameFixtures;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.UpdateGameLeagueRoundStatus;
using DomainCompetitionType = ProjectFootballSim.Calendar.Domain.Enums.CompetitionType;

namespace ProjectFootballSim.Calendar.Application.Features.ScheduleGameFixtures;

internal sealed class ScheduleGameFixturesCommandHandler(
    IScheduleGameFixturesRepository scheduleGameFixturesRepository,
    IUpdateGameLeagueRoundStatusCommandHandler updateGameLeagueRoundStatusCommandHandler) : IScheduleGameFixturesCommandHandler
{
    public async Task HandleAsync(ScheduleGameFixturesCommand command, CancellationToken cancellationToken)
    {
        if (command.Rounds.Count == 0)
            throw new ArgumentException("At least one round must be provided.");

        foreach (ScheduleGameFixturesRoundItem round in command.Rounds)
        {
            if (round.AvailableDaysOfWeek.Count == 0)
                continue;

            IReadOnlyList<GameFixture> roundFixtures = await scheduleGameFixturesRepository
                .GetProvisionalFixturesAsync(
                    command.UserId,
                    command.GameId,
                    round.GameCompetitionId,
                    MapCompetitionType(round.CompetitionType),
                    round.RoundId,
                    cancellationToken)
                .ConfigureAwait(false);

            if (roundFixtures.Count == 0)
                throw new InvalidOperationException($"No provisional fixtures found for round {round.RoundId}.");

            IReadOnlyList<DateTime> dates = GameFixtureDateCalculator.CalculateDates(
                round.BaseDate,
                round.AvailableDaysOfWeek,
                roundFixtures.Count);

            for (int i = 0; i < roundFixtures.Count; i++)
                roundFixtures[i].Schedule(dates[i]);
        }

        await scheduleGameFixturesRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);


        //TODO: async
        await updateGameLeagueRoundStatusCommandHandler.HandleAsync(new UpdateGameLeagueRoundStatusCommand(
                UserId: command.UserId,
                GameId: command.GameId,
                Rounds: command.Rounds.Select(round => new GameLeagueRoundReference(
                    GameLeagueId: round.GameCompetitionId,
                    Round: round.RoundId)).ToList(),
                Status: GameLeagueRoundStatusContract.Scheduled),
            cancellationToken).ConfigureAwait(false);
    }

    private static DomainCompetitionType MapCompetitionType(CompetitionType competitionType)
    {
        return competitionType switch
        {
            CompetitionType.League => DomainCompetitionType.League,
            _ => throw new ArgumentOutOfRangeException(nameof(competitionType), competitionType, "Unsupported competition type.")
        };
    }
}
