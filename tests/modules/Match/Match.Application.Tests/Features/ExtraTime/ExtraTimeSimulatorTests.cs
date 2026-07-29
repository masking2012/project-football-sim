using ProjectFootballSim.Match.Application.Common.Dtos;
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
        var home = new MatchTeamDto
        {
            Id = 1,
            Attack = 80,
            Defence = 70,
            Midfield = 75
        };
        var away = new MatchTeamDto
        {
            Id = 2,
            Attack = 75,
            Defence = 65,
            Midfield = 70
        };
        var settings = new MatchSettings
        {
            HasHomeAdvantage = false
        };

        foreach (var _ in Enumerable.Range(0, 100))
        {
            ScoreResult result = _sut.Play(home, away, settings);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.ExtraTime.Maximum);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.ExtraTime.Maximum);
        }
    }
}
