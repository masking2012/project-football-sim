using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Domain.Tests.ValueObjects;

internal sealed class GoalChancesSettingsTests
{
    [Test]
    public async Task RegularTimeShouldHaveExpectedValues()
    {
        var settings = GoalChancesSettings.RegularTime;

        using (Assert.Multiple())
        {
            await Assert.That(settings.Minimum).IsEqualTo(0);
            await Assert.That(settings.Maximum).IsEqualTo(22);
            await Assert.That(settings.BaseNumber).IsEqualTo(20);
        }
    }

    [Test]
    public async Task ExtraTimeShouldHaveExpectedValues()
    {
        var settings = GoalChancesSettings.ExtraTime;

        using (Assert.Multiple())
        {
            await Assert.That(settings.Minimum).IsEqualTo(0);
            await Assert.That(settings.Maximum).IsEqualTo(7);
            await Assert.That(settings.BaseNumber).IsEqualTo(6);
        }
    }

    [Test]
    public async Task ShouldBeEqualWhenPresetsAreTheSame()
    {
        var first = GoalChancesSettings.RegularTime;
        var second = GoalChancesSettings.RegularTime;

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
    public async Task ShouldNotBeEqualWhenPresetsDiffer()
    {
        var regular = GoalChancesSettings.RegularTime;
        var extra = GoalChancesSettings.ExtraTime;

        using (Assert.Multiple())
        {
            await Assert.That(regular.Equals(extra)).IsFalse();
            await Assert.That(regular == extra).IsFalse();
            await Assert.That(regular != extra).IsTrue();
        }
    }

    [Test]
    public async Task ShouldNotBeEqualToNonGoalChancesSettingsObject()
    {
        var settings = GoalChancesSettings.RegularTime;

        await Assert.That(settings.Equals((object)"RegularTime")).IsFalse();
    }
}
