using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Domain.Tests.ValueObjects;

internal sealed class AdvantageRatioTests
{
    [Test]
    public async Task NeutralShouldHaveValueOfOne()
    {
        await Assert.That(AdvantageRatio.Neutral.Value).IsEqualTo(1.00);
    }

    [Test]
    [Arguments(1.00)]
    [Arguments(1.50)]
    [Arguments(2.00)]
    public async Task ShouldCreateAdvantageRatioForValueWithinRange(double value)
    {
        var ratio = new AdvantageRatio(value);

        await Assert.That(ratio.Value).IsEqualTo(value);
    }

    [Test]
    [Arguments(0.99)]
    [Arguments(2.01)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    [Arguments(double.NegativeInfinity)]
    public async Task ShouldThrowWhenValueIsOutOfRangeOrNotFinite(double value)
    {
        await Assert.That(() => new AdvantageRatio(value)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ShouldBeEqualWhenValuesAreEqual()
    {
        var first = new AdvantageRatio(1.5);
        var second = new AdvantageRatio(1.5);

        using (Assert.Multiple())
        {
            await Assert.That(first.Equals(second)).IsTrue();
            await Assert.That(first.Equals((object)second)).IsTrue();
            await Assert.That(first == second).IsTrue();
            await Assert.That(first != second).IsFalse();
            await Assert.That(first.GetHashCode()).IsEqualTo(second.GetHashCode());
        }
    }

    [Test]
    public async Task ShouldNotBeEqualWhenValuesDiffer()
    {
        var first = new AdvantageRatio(1.5);
        var second = new AdvantageRatio(1.8);

        using (Assert.Multiple())
        {
            await Assert.That(first.Equals(second)).IsFalse();
            await Assert.That(first == second).IsFalse();
            await Assert.That(first != second).IsTrue();
        }
    }

    [Test]
    public async Task ShouldNotBeEqualToNonAdvantageRatioObject()
    {
        var ratio = new AdvantageRatio(1.5);
        await Assert.That(ratio.Equals((object)1.5)).IsFalse();
    }
}
