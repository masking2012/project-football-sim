using Moq;
using ProjectFootballSim.Calendar.Application.Features.ProvisionGameFixtures;
using ProjectFootballSim.Calendar.Contracts.Features.ProvisionGameFixtures;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;

namespace ProjectFootballSim.Calendar.Application.Tests.Features.ProvisionGameFixtures;

internal sealed class ProvisionGameFixturesCommandHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid GameLeagueId = Guid.NewGuid();

    private readonly Mock<IProvisionGameFixturesRepository> _repository = new();

    private ProvisionGameFixturesCommandHandler CreateSut() => new(_repository.Object);

    [Test]
    public async Task ShouldAddProvisionalLeagueFixturesAndSave()
    {
        var matchId = Guid.NewGuid();
        var scheduledAt = new DateTime(2026, 8, 2);
        var command = new ProvisionGameFixturesCommand(
            UserId,
            GameId,
            GameLeagueId,
            LeagueVersionId: 7,
            Fixtures: [new ProvisionGameFixtureItem(matchId, 1, 2, scheduledAt, 3)]);
        List<GameFixture> added = [];
        _repository
            .Setup(value => value.AddRange(It.IsAny<IEnumerable<GameFixture>>()))
            .Callback<IEnumerable<GameFixture>>(fixtures => added.AddRange(fixtures));

        await CreateSut().HandleAsync(command, CancellationToken.None).ConfigureAwait(false);

        var fixture = added.Single();
        using (Assert.Multiple())
        {
            await Assert.That(fixture.MatchId).IsEqualTo(matchId);
            await Assert.That(fixture.GameCompetitionId).IsEqualTo(GameLeagueId);
            await Assert.That(fixture.CompetitionVersionId).IsEqualTo(7);
            await Assert.That(fixture.CompetitionType).IsEqualTo(CompetitionType.League);
            await Assert.That(fixture.Status).IsEqualTo(FixtureStatus.Provisional);
            await Assert.That(fixture.HomeTeamId).IsEqualTo(1);
            await Assert.That(fixture.AwayTeamId).IsEqualTo(2);
            await Assert.That(fixture.ScheduledAt).IsEqualTo(scheduledAt);
            await Assert.That(fixture.RoundId).IsEqualTo(3);
        }

        _repository.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
