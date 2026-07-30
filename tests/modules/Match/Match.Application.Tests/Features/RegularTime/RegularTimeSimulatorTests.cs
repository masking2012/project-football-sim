using ProjectFootballSim.Match.Application.Common.Dtos;
using ProjectFootballSim.Match.Application.Common.Services;
using ProjectFootballSim.Matches.Application.Features.RegularTime;
using ProjectFootballSim.Matches.Domain.ValueObjects;

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

    [Test]
    public async Task WinLossRatioAsync()
    {
        var bayernMunchen = new MatchTeamDto
        {
            Id = 1,
            Attack = 90,
            Midfield = 85,
            Defence = 83
        };
        var dynamoKyiv = new MatchTeamDto
        {
            Id = 2,
            Attack = 59,
            Midfield = 63,
            Defence = 61
        };
        MatchSettings settings = new MatchSettings
        {
            HasHomeAdvantage = false
        };

        int homeWins = 0;
        int draws = 0;
        int awayWins = 0;

        foreach (var _ in Enumerable.Range(0, 1000))
        {
            ScoreResult result = _sut.Play(bayernMunchen, dynamoKyiv, settings);

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
