using ProjectFootballSim.Match.Application.Common.Services;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Match.Application.Tests.Common.Services;

internal sealed class ChancesCalculatorTests
{
    private readonly ChancesCalculator _sut = new();

    [Test]
    public async Task ShouldCalculateChancesAsync()
    {
        Team attacking = new Team(Guid.NewGuid(), 50, 50, 50);
        Team defending = new Team(Guid.NewGuid(), 50, 50, 50);
        Possession possession = new Possession(0.5);
        AdvantageRatio advantageRatio = new AdvantageRatio(1.0);
        GoalChancesSettings goalChancesSettings = GoalChancesSettings.RegularTime;

        foreach (var _ in Enumerable.Range(0, 100))
        {
            int chances = _sut.Calculate(attacking, defending, possession, advantageRatio, goalChancesSettings);
            await Assert.That(chances).IsBetween(goalChancesSettings.Minimum, goalChancesSettings.Maximum);
        }
    }
}
