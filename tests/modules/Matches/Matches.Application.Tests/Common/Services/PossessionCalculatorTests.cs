using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Domain.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Tests.Common.Services;

internal sealed class PossessionCalculatorTests
{
    private readonly PossessionCalculator _sut = new();

    [Test]
    [Arguments(50, 50)]
    [Arguments(100, 1)]
    [Arguments(1, 100)]
    [Arguments(100, 100)]
    [Arguments(1, 1)]
    public async Task ShouldCalculatePossessionAsync(int homeMidfield, int awayMidfield)
    {
        Possession possession = _sut.Calculate(homeMidfield, awayMidfield);
        await Assert.That(possession.Value).IsBetween(IPossessionCalculator.MinPossession, IPossessionCalculator.MaxPossession);
        await Assert.That(possession.OpponentPossession.Value).IsEqualTo(1 - possession.Value);
    }
}
