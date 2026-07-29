using ProjectFootballSim.Match.Application.Common.Dtos;
using ProjectFootballSim.Match.Application.Features.Penalty;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Match.Application.Tests.Features.Penalty;

internal sealed class PenaltySimulatorTests
{
    [Test]
    public async Task ShouldPlayPenaltyAsync()
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

        foreach (var _ in Enumerable.Range(0, 100))
        {
            ScoreResult result = PenaltySimulator.Play(home, away);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0);
            await Assert.That(result.HomeScore).IsNotEqualTo(result.AwayScore);
        }
    }
}
