using ProjectFootballSim.Match.Application.Features.Penalty;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Match.Application.Tests.Features.Penalty;

internal sealed class PenaltySimulatorTests
{
    [Test]
    public async Task ShouldPlayPenaltyAsync()
    {
        Team home = new(Guid.NewGuid(), 80, 70, 75);
        Team away = new(Guid.NewGuid(), 75, 65, 70);

        foreach(var _ in Enumerable.Range(0, 500))
        {
            ScoreResult result = PenaltySimulator.Play(home, away);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0);
            await Assert.That(result.HomeScore).IsNotEqualTo(result.AwayScore);
        }
    }
}
