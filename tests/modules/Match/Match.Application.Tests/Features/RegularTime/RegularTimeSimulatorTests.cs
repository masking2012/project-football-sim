using ProjectFootballSim.Match.Application.Common.Dtos;
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
