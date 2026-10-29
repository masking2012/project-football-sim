using Microsoft.Extensions.Logging;
using Moq;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Features.Penalty;
using ProjectFootballSim.Matches.Application.Tests.Helpers;

namespace ProjectFootballSim.Matches.Application.Tests.Features.Penalty;

internal sealed class SimulatePenaltyShootoutCommandHandlerTests
{
    private readonly Mock<ILogger<SimulatePenaltyShootoutCommandHandler>> _loggerMock = new();
    private readonly SimulatePenaltyShootoutCommandHandler _sut;

    public SimulatePenaltyShootoutCommandHandlerTests()
    {
        _sut = new SimulatePenaltyShootoutCommandHandler(_loggerMock.Object, new TestRandomNumberGenerator(42));
    }

    [Test]
    public async Task ShouldPlayPenalty()
    {
        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);

        foreach (var _ in Enumerable.Range(0, 100))
        {
            var command = new SimulatePenaltyShootoutCommand(home, away);
            ScoreResultDto result = _sut.Handle(command);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0);
            await Assert.That(result.HomeScore).IsNotEqualTo(result.AwayScore);
        }
    }

    [Test]
    public async Task ShouldStopAfterFifthHomeKickClinchesShootout()
    {
        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);
        double[] kickOutcomes = [0.0, 0.0, 0.0, 0.99, 0.99, 0.0, 0.0, 0.99, 0.0];
        var simulator = new SimulatePenaltyShootoutCommandHandler(
            _loggerMock.Object,
            new TestRandomNumberGenerator(kickOutcomes));

        var command = new SimulatePenaltyShootoutCommand(home, away);
        ScoreResultDto result = simulator.Handle(command);

        await Assert.That(result.HomeScore).IsEqualTo(4);
        await Assert.That(result.AwayScore).IsEqualTo(2);
    }

    [Test]
    public async Task ShouldStopAfterAwayTeamClinchesShootout()
    {
        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);
        double[] kickOutcomes = [0.99, 0.0, 0.99, 0.0, 0.99, 0.0];
        var simulator = new SimulatePenaltyShootoutCommandHandler(
            _loggerMock.Object,
            new TestRandomNumberGenerator(kickOutcomes));

        ScoreResultDto result = simulator.Handle(new SimulatePenaltyShootoutCommand(home, away));

        await Assert.That(result.HomeScore).IsEqualTo(0);
        await Assert.That(result.AwayScore).IsEqualTo(3);
    }

    [Test]
    public async Task ShouldResolveTiedShootoutInSuddenDeath()
    {
        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);
        double[] kickOutcomes = [
            0.0, 0.0,
            0.0, 0.0,
            0.0, 0.0,
            0.0, 0.0,
            0.0, 0.0,
            0.0, 0.99];
        var simulator = new SimulatePenaltyShootoutCommandHandler(
            _loggerMock.Object,
            new TestRandomNumberGenerator(kickOutcomes));

        ScoreResultDto result = simulator.Handle(new SimulatePenaltyShootoutCommand(home, away));

        await Assert.That(result.HomeScore).IsEqualTo(6);
        await Assert.That(result.AwayScore).IsEqualTo(5);
    }
}
