using ProjectFootballSim.Match.Application.Common.Services;
using ProjectFootballSim.Match.Application.Features.RegularTime;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Match.Application.Tests.Features.RegularTime;

internal sealed class RegularTimeSimulatorTests
{
    private readonly RegularTimeSimulator _sut;

    public RegularTimeSimulatorTests()
    {
        _sut = new RegularTimeSimulator(new PossessionCalculator(), new ChancesCalculator(), new GoalsCalculator());
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

        foreach (var _ in Enumerable.Range(0, 100))
        {
            ScoreResult result = _sut.Play(home, away, settings);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.RegularTime.Maximum);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.RegularTime.Maximum);
        }
    }
}
