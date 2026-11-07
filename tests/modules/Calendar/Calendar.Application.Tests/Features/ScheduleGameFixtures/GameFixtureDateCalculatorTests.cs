using ProjectFootballSim.Calendar.Application.Features.ScheduleGameFixtures;

namespace ProjectFootballSim.Calendar.Application.Tests.Features.ScheduleGameFixtures;

internal sealed class GameFixtureDateCalculatorTests
{
    private static readonly DateTime BaseDate = new(2026, 11, 8);

    private static readonly IReadOnlySet<DayOfWeek> AvailableDays =
        new HashSet<DayOfWeek> { DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday };

    [Test]
    public async Task ShouldSpreadFixturesEvenlyAcrossAvailableDates()
    {
        var dates = GameFixtureDateCalculator.CalculateDates(BaseDate, AvailableDays, 8);

        await Assert.That(dates).IsEquivalentTo(
        [
            new DateTime(2026, 11, 6), new DateTime(2026, 11, 6),
            new DateTime(2026, 11, 7), new DateTime(2026, 11, 7),
            new DateTime(2026, 11, 8), new DateTime(2026, 11, 8),
            new DateTime(2026, 11, 9), new DateTime(2026, 11, 9)
        ]);
    }

    [Test]
    public async Task ShouldReturnOneFixturePerDateWhenFixtureCountEqualsAvailableDays()
    {
        var dates = GameFixtureDateCalculator.CalculateDates(BaseDate, AvailableDays, 4);

        await Assert.That(dates).IsEquivalentTo(
        [
            new DateTime(2026, 11, 6),
            new DateTime(2026, 11, 7),
            new DateTime(2026, 11, 8),
            new DateTime(2026, 11, 9)
        ]);
    }

    [Test]
    public async Task ShouldGiveRemainingFixturesToEarliestDates()
    {
        var dates = GameFixtureDateCalculator.CalculateDates(BaseDate, AvailableDays, 6);

        await Assert.That(dates).IsEquivalentTo(
        [
            new DateTime(2026, 11, 6), new DateTime(2026, 11, 6),
            new DateTime(2026, 11, 7), new DateTime(2026, 11, 7),
            new DateTime(2026, 11, 8),
            new DateTime(2026, 11, 9)
        ]);
    }

    [Test]
    public async Task ShouldReturnDatesInChronologicalOrder()
    {
        var dates = GameFixtureDateCalculator.CalculateDates(BaseDate, AvailableDays, 8);

        await Assert.That(dates).IsInOrder();
    }

    [Test]
    public async Task ShouldReturnEmptyListWhenFixtureCountIsZero()
    {
        var dates = GameFixtureDateCalculator.CalculateDates(BaseDate, AvailableDays, 0);

        await Assert.That(dates).IsEmpty();
    }

    [Test]
    public async Task ShouldThrowWhenBaseDateIsNotSunday()
    {
        await Assert.That(() => GameFixtureDateCalculator.CalculateDates(new DateTime(2026, 11, 9), AvailableDays, 4))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldThrowWhenNoAvailableDaysAreProvided()
    {
        await Assert.That(() => GameFixtureDateCalculator.CalculateDates(BaseDate, new HashSet<DayOfWeek>(), 4))
            .Throws<ArgumentException>();
    }
}
