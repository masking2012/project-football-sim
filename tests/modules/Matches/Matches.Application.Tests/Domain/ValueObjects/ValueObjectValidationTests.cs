using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Tests.Domain.ValueObjects;

internal sealed class ValueObjectValidationTests
{
    [Test]
    public async Task PossessionShouldRejectNonFiniteValuesAsync()
    {
        await Assert.That(() => new Possession(double.NaN)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => new Possession(double.PositiveInfinity)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => new Possession(double.NegativeInfinity)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task AdvantageRatioShouldRejectNonFiniteValuesAsync()
    {
        await Assert.That(() => new AdvantageRatio(double.NaN)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => new AdvantageRatio(double.PositiveInfinity)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => new AdvantageRatio(double.NegativeInfinity)).Throws<ArgumentOutOfRangeException>();
    }
}
