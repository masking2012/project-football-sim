using Microsoft.Extensions.Logging;
using Moq;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Application.Features.RegularTime;
using ProjectFootballSim.Matches.Application.Tests.Helpers;
using ProjectFootballSim.Matches.Domain.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Tests.Features.RegularTime;

internal sealed class SimulateRegularTimeCommandHandlerTests
{
    private readonly Mock<ILogger<SimulateRegularTimeCommandHandler>> _loggerMock = new();
    private readonly SimulateRegularTimeCommandHandler _sut;

    public SimulateRegularTimeCommandHandlerTests()
    {
        var random = new TestRandomNumberGenerator(42);
        _sut = new SimulateRegularTimeCommandHandler(
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
            ScoreResultDto result = _sut.Handle(new SimulateRegularTimeCommand(home, away, settings));

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
            ScoreResultDto result = _sut.Handle(new SimulateRegularTimeCommand(bayernMunchen, dynamoKyiv, settings));

            if (result.HomeScore > result.AwayScore)
                homeWins++;
            else if (result.HomeScore < result.AwayScore)
                awayWins++;
            else
                draws++;
        }

        await Assert.That(homeWins).IsGreaterThan(awayWins);
    }

    [Test]
    public async Task ShouldRejectNullCommandAsync()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => _sut.Handle(null!));

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.ParamName).IsEqualTo("command");
    }

    [Test]
    public async Task ShouldApplyHomeAdvantageToHomeChancesAsync()
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

        var sut = new SimulateRegularTimeCommandHandler(
            _loggerMock.Object,
            possessionCalculator.Object,
            chancesCalculator.Object,
            goalsCalculator.Object);

        var home = new MatchTeamDto(1, 80, 70, 75);
        var away = new MatchTeamDto(2, 75, 65, 70);
        sut.Handle(new SimulateRegularTimeCommand(home, away, new MatchSettingsDto(HasHomeAdvantage: true)));

        chancesCalculator.Verify(calculator => calculator.Calculate(
            It.Is<MatchTeam>(team => team.Id == home.Id),
            It.Is<MatchTeam>(team => team.Id == away.Id),
            It.IsAny<Possession>(),
            It.Is<AdvantageRatio>(ratio => ratio == new AdvantageRatio(1.1)),
            GoalChancesSettings.RegularTime), Times.Once);
    }
}
