using Moq;
using ProjectFootballSim.Calendar.Application.Features.ScheduleGameFixtures;
using ProjectFootballSim.Calendar.Contracts.Features.ScheduleGameFixtures;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Shared.Features.Events;
using ContractCompetitionType = ProjectFootballSim.Calendar.Contracts.Features.ScheduleGameFixtures.CompetitionType;
using DomainCompetitionType = ProjectFootballSim.Calendar.Domain.Enums.CompetitionType;

namespace ProjectFootballSim.Calendar.Application.Tests.Features.ScheduleGameFixtures;

internal sealed class ScheduleGameFixturesCommandHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid GameCompetitionId = Guid.NewGuid();
    private static readonly DateTime BaseDate = new(2026, 11, 8);
    private static readonly IReadOnlySet<DayOfWeek> AvailableDays =
        new HashSet<DayOfWeek> { DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday };

    private readonly Mock<IScheduleGameFixturesRepository> _repository = new();
    private readonly Mock<IEventPublisher> _eventPublisher = new();

    private ScheduleGameFixturesCommandHandler CreateSut() => new(_repository.Object, _eventPublisher.Object);

    [Test]
    public async Task ShouldThrowWhenNoRoundsProvided()
    {
        var command = new ScheduleGameFixturesCommand(UserId, GameId, []);

        Func<Task> act = () => CreateSut().HandleAsync(command, CancellationToken.None);

        await Assert.That(act).Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldThrowWhenNoProvisionalFixturesFound()
    {
        SetupFixtures([]);
        var command = CreateCommand(roundId: 1, availableDays: AvailableDays);

        Func<Task> act = () => CreateSut().HandleAsync(command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldScheduleFixturesAndPublishEvent()
    {
        var fixtures = new List<GameFixture> { CreateFixture(), CreateFixture() };
        SetupFixtures(fixtures);
        var command = CreateCommand(roundId: 1, availableDays: AvailableDays);

        await CreateSut().HandleAsync(command, CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(fixtures).All(fixture => fixture.Status == FixtureStatus.Scheduled);
            await Assert.That(fixtures).All(fixture => fixture.ScheduledAt >= BaseDate);
        }

        _repository.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisher.Verify(value => value.PublishAsync(
            It.Is<ScheduleGameFixturesEvent>(@event => @event.UserId == UserId && @event.GameId == GameId && @event.Versions.Count == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ShouldSkipRoundsWithoutAvailableDaysAndNotPublish()
    {
        var command = CreateCommand(roundId: 1, availableDays: new HashSet<DayOfWeek>());

        await CreateSut().HandleAsync(command, CancellationToken.None).ConfigureAwait(false);

        _repository.Verify(value => value.GetProvisionalFixturesAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<DomainCompetitionType>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _eventPublisher.Verify(value => value.PublishAsync(It.IsAny<ScheduleGameFixturesEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static ScheduleGameFixturesCommand CreateCommand(int roundId, IReadOnlySet<DayOfWeek> availableDays) =>
        new(
            UserId,
            GameId,
            [new ScheduleGameFixturesRoundItem(GameCompetitionId, ContractCompetitionType.League, roundId, BaseDate, availableDays)]);

    private static GameFixture CreateFixture() =>
        new(
            matchId: Guid.NewGuid(),
            gameCompetitionId: GameCompetitionId,
            competitionVersionId: 1,
            competitionType: DomainCompetitionType.League,
            homeTeamId: 1,
            awayTeamId: 2,
            scheduledAt: BaseDate,
            status: FixtureStatus.Provisional,
            roundId: 1,
            userId: UserId,
            gameId: GameId);

    private void SetupFixtures(IReadOnlyList<GameFixture> fixtures) =>
        _repository
            .Setup(value => value.GetProvisionalFixturesAsync(
                UserId,
                GameId,
                GameCompetitionId,
                DomainCompetitionType.League,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixtures);
}
