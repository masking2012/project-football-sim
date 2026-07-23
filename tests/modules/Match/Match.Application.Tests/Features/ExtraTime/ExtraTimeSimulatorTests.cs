using ProjectFootballSim.Match.Application.Common.Services;
using ProjectFootballSim.Match.Application.Features.ExtraTime;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Match.Application.Tests.Features.ExtraTime;

internal sealed class ExtraTimeSimulatorTests
{
    private readonly ExtraTimeSimulator _sut;

    public ExtraTimeSimulatorTests()
    {
        _sut = new ExtraTimeSimulator(new PossessionCalculator(), new ChancesCalculator(), new GoalsCalculator());
    }

    [Test]
    public async Task ShouldPlayGameAsync()
    {
        Team home = new(Guid.NewGuid(), 80, 70, 75);
        Team away = new(Guid.NewGuid(), 75, 65, 70);
        MatchSettings settings = new MatchSettings
        {
            HasHomeAdvantage = false
        };

        foreach (var _ in Enumerable.Range(0, 500))
        {
            ScoreResult result = _sut.Play(home, away, settings);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.ExtraTime.Maximum);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.ExtraTime.Maximum);
        }
    }
}
