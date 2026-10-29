using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Application.Tests.Helpers;
using ProjectFootballSim.Matches.Domain.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Tests.Common.Services;

internal sealed class PossessionCalculatorTests
{
    private readonly PossessionCalculator _sut = new(new TestRandomNumberGenerator(42));

    [Test]
    [Arguments(50, 50)]
    [Arguments(100, 1)]
    [Arguments(1, 100)]
    [Arguments(100, 100)]
    [Arguments(1, 1)]
    public async Task ShouldCalculatePossession(int homeMidfield, int awayMidfield)
    {
        Possession possession = _sut.Calculate(homeMidfield, awayMidfield);
        await Assert.That(possession.Value).IsBetween(IPossessionCalculator.MinPossession, IPossessionCalculator.MaxPossession);
        await Assert.That(possession.OpponentPossession.Value).IsEqualTo(1 - possession.Value);
    }

    [Test]
    public async Task ShouldClampPossessionToMaximum()
    {
        var calculator = new PossessionCalculator(new TestRandomNumberGenerator([0.0]));

        Possession possession = calculator.Calculate(99, 1);

        await Assert.That(possession.Value).IsEqualTo(IPossessionCalculator.MaxPossession);
    }

    [Test]
    public async Task ShouldClampPossessionToMinimum()
    {
        var calculator = new PossessionCalculator(new TestRandomNumberGenerator([0.99]));

        Possession possession = calculator.Calculate(1, 99);

        await Assert.That(possession.Value).IsEqualTo(IPossessionCalculator.MinPossession);
    }
}
