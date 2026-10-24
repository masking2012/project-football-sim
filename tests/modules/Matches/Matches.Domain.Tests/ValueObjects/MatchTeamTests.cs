using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Domain.Tests.ValueObjects;

internal sealed class MatchTeamTests
{
    [Test]
    public async Task ShouldExposeConstructorValues()
    {
        var team = new MatchTeam(7, 50, 60, 70);

        using (Assert.Multiple())
        {
            await Assert.That(team.Id).IsEqualTo(7);
            await Assert.That(team.Attack).IsEqualTo(50);
            await Assert.That(team.Defence).IsEqualTo(60);
            await Assert.That(team.Midfield).IsEqualTo(70);
        }
    }

    [Test]
    [Arguments(1, 1, 1)]
    [Arguments(99, 99, 99)]
    public async Task ShouldAcceptRatingsAtBounds(int attack, int defence, int midfield)
    {
        var team = new MatchTeam(1, attack, defence, midfield);

        using (Assert.Multiple())
        {
            await Assert.That(team.Attack).IsEqualTo(attack);
            await Assert.That(team.Defence).IsEqualTo(defence);
            await Assert.That(team.Midfield).IsEqualTo(midfield);
        }
    }

    [Test]
    [Arguments(0, 50, 50)]
    [Arguments(100, 50, 50)]
    public async Task ShouldThrowWhenAttackIsOutOfRange(int attack, int defence, int midfield)
    {
        await Assert.That(() => new MatchTeam(1, attack, defence, midfield)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(50, 0, 50)]
    [Arguments(50, 100, 50)]
    public async Task ShouldThrowWhenDefenceIsOutOfRange(int attack, int defence, int midfield)
    {
        await Assert.That(() => new MatchTeam(1, attack, defence, midfield)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(50, 50, 0)]
    [Arguments(50, 50, 100)]
    public async Task ShouldThrowWhenMidfieldIsOutOfRange(int attack, int defence, int midfield)
    {
        await Assert.That(() => new MatchTeam(1, attack, defence, midfield)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ShouldBeEqualWhenAllPropertiesMatch()
    {
        var first = new MatchTeam(3, 40, 50, 60);
        var second = new MatchTeam(3, 40, 50, 60);

        await Assert.That(first).IsEqualTo(second);
    }
}
