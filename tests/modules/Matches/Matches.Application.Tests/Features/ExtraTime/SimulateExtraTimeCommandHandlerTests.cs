using Microsoft.Extensions.Logging;
using Moq;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Application.Features.ExtraTime;
using ProjectFootballSim.Matches.Application.Tests.Helpers;
using ProjectFootballSim.Matches.Domain.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Tests.Features.ExtraTime;

internal sealed class SimulateExtraTimeCommandHandlerTests
{
    private readonly Mock<ILogger<SimulateExtraTimeCommandHandler>> _loggerMock = new();
    private readonly SimulateExtraTimeCommandHandler _sut;

    public SimulateExtraTimeCommandHandlerTests()
    {
        var random = new TestRandomNumberGenerator(42);
        _sut = new SimulateExtraTimeCommandHandler(
            _loggerMock.Object,
            new PossessionCalculator(random),
            new ChancesCalculator(random),
            new GoalsCalculator(random));
    }

    [Test]
    public async Task ShouldPlayGameAsync()
    {
        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);
        var settings = new MatchSettingsDto(HasHomeAdvantage: false);

        foreach (var _ in Enumerable.Range(0, 100))
        {
            var command = new SimulateExtraTimeCommand(home, away, settings);
            ScoreResultDto result = _sut.Handle(command);

            await Assert.That(result.HomeScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.ExtraTime.Maximum);
            await Assert.That(result.AwayScore).IsGreaterThanOrEqualTo(0).And.IsLessThanOrEqualTo(GoalChancesSettings.ExtraTime.Maximum);
        }
    }

    [Test]
    public async Task ShouldApplyHomeAdvantageToExtraTimeChancesAsync()
    {
        var possessionCalculator = new Mock<IPossessionCalculator>();
        possessionCalculator
            .Setup(calculator => calculator.Calculate(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new Possession(0.5));

        var chancesCalculator = new Mock<IChancesCalculator>();
        chancesCalculator
            .Setup(calculator => calculator.Calculate(
                It.IsAny<MatchTeam>(),
                It.IsAny<MatchTeam>(),
                It.IsAny<Possession>(),
                It.IsAny<AdvantageRatio>(),
                It.IsAny<GoalChancesSettings>()))
            .Returns(1);

        var goalsCalculator = new Mock<IGoalsCalculator>();
        goalsCalculator
            .Setup(calculator => calculator.Calculate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(0);

        var sut = new SimulateExtraTimeCommandHandler(
            _loggerMock.Object,
            possessionCalculator.Object,
            chancesCalculator.Object,
            goalsCalculator.Object);

        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);
        sut.Handle(new SimulateExtraTimeCommand(home, away, new MatchSettingsDto(HasHomeAdvantage: true)));

        chancesCalculator.Verify(calculator => calculator.Calculate(
            It.Is<MatchTeam>(team => team.Id == home.Id),
            It.Is<MatchTeam>(team => team.Id == away.Id),
            It.IsAny<Possession>(),
            It.Is<AdvantageRatio>(ratio => ratio == new AdvantageRatio(1.1)),
            GoalChancesSettings.ExtraTime), Times.Once);
    }
}
