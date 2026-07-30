using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Application.Features.RegularTime;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Tests.Features.RegularTime;

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
        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);
        var settings = new MatchSettingsDto(HasHomeAdvantage: false);

        foreach (var _ in Enumerable.Range(0, 100))
        {
            ScoreResultDto result = _sut.Play(home, away, settings);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.RegularTime.Maximum);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.RegularTime.Maximum);
        }
    }

    [Test]
    public async Task WinLossRatioAsync()
    {
        var bayernMunchen = new MatchTeamDto(1, 90, 83, 85);
        var dynamoKyiv = new MatchTeamDto(2, 59, 61, 63);
        var settings = new MatchSettingsDto(HasHomeAdvantage: false);

        int homeWins = 0;
        int draws = 0;
        int awayWins = 0;

        foreach (var _ in Enumerable.Range(0, 1000))
        {
            ScoreResultDto result = _sut.Play(bayernMunchen, dynamoKyiv, settings);

            if (result.HomeScore > result.AwayScore)
                homeWins++;
            else if (result.HomeScore < result.AwayScore)
                awayWins++;
            else
                draws++;
        }

        await Assert.That(homeWins).IsGreaterThan(awayWins);
    }
}
