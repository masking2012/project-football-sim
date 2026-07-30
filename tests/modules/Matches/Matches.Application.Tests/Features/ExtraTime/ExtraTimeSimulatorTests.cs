using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Application.Features.ExtraTime;
using ProjectFootballSim.Matches.Domain.ValueObjects;

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
        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);
        var settings = new MatchSettingsDto(HasHomeAdvantage: false);

        foreach (var _ in Enumerable.Range(0, 100))
        {
            ScoreResultDto result = _sut.Play(home, away, settings);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.ExtraTime.Maximum);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.ExtraTime.Maximum);
        }
    }
}
