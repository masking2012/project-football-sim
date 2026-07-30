using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Features.Penalty;

namespace ProjectFootballSim.Matches.Application.Tests.Features.Penalty;

internal sealed class PenaltySimulatorTests
{
    [Test]
    public async Task ShouldPlayPenaltyAsync()
    {
        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);

        foreach (var _ in Enumerable.Range(0, 100))
        {
            ScoreResultDto result = PenaltySimulator.Play(home, away);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0);
            await Assert.That(result.HomeScore).IsNotEqualTo(result.AwayScore);
        }
    }
}
