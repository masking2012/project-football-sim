using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Application.Tests.Helpers;

namespace ProjectFootballSim.Matches.Application.Tests.Common.Services;

internal sealed class GoalsCalculatorTests
{
    private readonly GoalsCalculator _sut = new(new TestRandomNumberGenerator(42));

    [Test]
    public async Task ShouldCalculateGoals()
    {
        foreach (var _ in Enumerable.Range(0, 100))
        {
            int goals = _sut.Calculate(50, 50, 10);
            await Assert.That(goals).IsBetween(0, 10);
        }
    }

    [Test]
    public async Task ShouldReturnZeroWhenThereAreNoChances()
    {
        int goals = _sut.Calculate(50, 50, 0);

        await Assert.That(goals).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldScoreEveryChanceWhenRandomValuesAreZero()
    {
        var calculator = new GoalsCalculator(new TestRandomNumberGenerator([0.0, 0.0, 0.0, 0.0, 0.0, 0.0]));

        int goals = calculator.Calculate(99, 1, 3);

        await Assert.That(goals).IsEqualTo(3);
    }
}
