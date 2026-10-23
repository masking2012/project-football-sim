using Microsoft.Extensions.Logging;
using Moq;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Tests.Common;
using ProjectFootballSim.Matches.Application.Features.Penalty;

namespace ProjectFootballSim.Matches.Application.Tests.Features.Penalty;

internal sealed class PenaltySimulatorTests
{
    private readonly Mock<ILogger<SimulatePenaltyShootoutCommand>> _loggerMock = new();
    private readonly SimulatePenaltyShootoutCommand _sut;

    public PenaltySimulatorTests()
    {
        _sut = new SimulatePenaltyShootoutCommand(_loggerMock.Object, new TestRandomNumberGenerator(42));
    }

    [Test]
    public async Task ShouldPlayPenaltyAsync()
    {
        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);

        foreach (var _ in Enumerable.Range(0, 100))
        {
            ScoreResultDto result = _sut.Handle(home, away);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0);
            await Assert.That(result.HomeScore).IsNotEqualTo(result.AwayScore);
        }
    }

    [Test]
    public async Task ShouldStopAfterFifthHomeKickClinchesShootoutAsync()
    {
        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);
        double[] kickOutcomes = [0.0, 0.0, 0.0, 0.99, 0.99, 0.0, 0.0, 0.99, 0.0];
        var simulator = new SimulatePenaltyShootoutCommand(
            _loggerMock.Object,
            new TestRandomNumberGenerator(kickOutcomes));

        ScoreResultDto result = simulator.Handle(home, away);

        await Assert.That(result.HomeScore).IsEqualTo(4);
        await Assert.That(result.AwayScore).IsEqualTo(2);
    }
}
