using ProjectFootballSim.Calendar.Contracts.Features.ProvisionGameFixtures;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;

namespace ProjectFootballSim.Calendar.Application.Features.ProvisionGameFixtures;

internal sealed class ProvisionGameFixturesCommandHandler(
    IProvisionGameFixturesRepository provisionGameFixturesRepository) : IProvisionGameFixturesCommandHandler
{
    public async Task HandleAsync(ProvisionGameFixturesCommand command, CancellationToken cancellationToken)
    {
        IEnumerable<GameFixture> gameFixtures = command.Fixtures.Select(fixture => new GameFixture(
            matchId: fixture.MatchId,
            gameCompetitionId: command.GameLeagueId,
            competitionVersionId: command.LeagueVersionId,
            competitionType: CompetitionType.League,
            homeTeamId: fixture.HomeTeamId,
            awayTeamId: fixture.AwayTeamId,
            scheduledAt: fixture.ScheduledAt,
            status: FixtureStatus.Provisional,
            roundId: fixture.RoundId,
            userId: command.UserId,
            gameId: command.GameId));

        provisionGameFixturesRepository.AddRange(gameFixtures);
        await provisionGameFixturesRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
