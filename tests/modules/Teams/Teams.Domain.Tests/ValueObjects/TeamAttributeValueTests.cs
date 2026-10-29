using ProjectFootballSim.Teams.Domain.ValueObjects;

namespace ProjectFootballSim.Teams.Domain.Tests.ValueObjects;

internal sealed class TeamAttributeValueTests
{
    [Test]
    [Arguments(1)]
    [Arguments(99)]
    public async Task ShouldAcceptValuesAtBounds(int value)
    {
        var attribute = new TeamAttributeValue(value);

        await Assert.That(attribute.Value).IsEqualTo(value);
    }

    [Test]
    [Arguments(0)]
    [Arguments(100)]
    public async Task ShouldRejectValuesOutsideBounds(int value)
    {
        await Assert.That(() => new TeamAttributeValue(value)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ShouldBeEqualWhenValuesMatch()
    {
        var first = new TeamAttributeValue(75);
        var second = new TeamAttributeValue(75);

        using (Assert.Multiple())
        {
            await Assert.That(first).IsEqualTo(second);
            await Assert.That(first == second).IsTrue();
            await Assert.That(first.GetHashCode()).IsEqualTo(second.GetHashCode());
        }
    }

    [Test]
    public async Task ShouldNotBeEqualWhenValuesDiffer()
    {
        var first = new TeamAttributeValue(75);
        var second = new TeamAttributeValue(76);

        using (Assert.Multiple())
        {
            await Assert.That(first).IsNotEqualTo(second);
            await Assert.That(first != second).IsTrue();
        }
    }
}
