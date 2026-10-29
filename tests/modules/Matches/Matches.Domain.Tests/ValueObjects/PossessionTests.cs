using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Domain.Tests.ValueObjects;

internal sealed class PossessionTests
{
    [Test]
    [Arguments(0.00)]
    [Arguments(0.50)]
    [Arguments(1.00)]
    public async Task ShouldCreatePossessionForValueWithinRange(double value)
    {
        var possession = new Possession(value);

        await Assert.That(possession.Value).IsEqualTo(value);
    }

    [Test]
    [Arguments(-0.01)]
    [Arguments(1.01)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    [Arguments(double.NegativeInfinity)]
    public async Task ShouldThrowWhenValueIsOutOfRangeOrNotFinite(double value)
    {
        await Assert.That(() => new Possession(value)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(0.25, 0.75)]
    [Arguments(0.00, 1.00)]
    [Arguments(1.00, 0.00)]
    public async Task ShouldReturnComplementAsOpponentPossession(double value, double expectedOpponent)
    {
        var possession = new Possession(value);

        await Assert.That(possession.OpponentPossession.Value).IsEqualTo(expectedOpponent);
    }

    [Test]
    public async Task ShouldBeEqualWhenValuesAreEqual()
    {
        var first = new Possession(0.4);
        var second = new Possession(0.4);

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
        var first = new Possession(0.4);
        var second = new Possession(0.6);

        using (Assert.Multiple())
        {
            await Assert.That(first.Equals(second)).IsFalse();
            await Assert.That(first == second).IsFalse();
            await Assert.That(first != second).IsTrue();
        }
    }

    [Test]
    public async Task ShouldNotBeEqualToNonPossessionObject()
    {
        var possession = new Possession(0.4);

        await Assert.That(possession.Equals((object)0.4)).IsFalse();
    }
}
