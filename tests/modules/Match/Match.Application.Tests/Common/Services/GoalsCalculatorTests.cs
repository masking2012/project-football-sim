using ProjectFootballSim.Match.Application.Common.Services;

namespace ProjectFootballSim.Match.Application.Tests.Common.Services;

internal sealed class GoalsCalculatorTests
{
    private readonly GoalsCalculator _sut = new();

    [Test]
    public async Task ShouldCalculateGoalsAsync()
    {
        foreach (var _ in Enumerable.Range(0, 100))
        {
            int goals = _sut.Calculate(50, 50, 10);
            await Assert.That(goals).IsBetween(0, 10);
        }
    }
}
