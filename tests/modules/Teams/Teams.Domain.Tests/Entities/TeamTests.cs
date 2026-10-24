using ProjectFootballSim.Teams.Domain.Entities;
using ProjectFootballSim.Teams.Domain.ValueObjects;

namespace ProjectFootballSim.Teams.Domain.Tests.Entities;

internal sealed class TeamTests
{
    [Test]
    public async Task ShouldExposeConstructorValues()
    {
        var team = CreateTeam();

        using (Assert.Multiple())
        {
            await Assert.That(team.Id).IsEqualTo(7);
            await Assert.That(team.Name).IsEqualTo("Test Team");
            await Assert.That(team.Attack.Value).IsEqualTo(80);
            await Assert.That(team.Midfield.Value).IsEqualTo(70);
            await Assert.That(team.Defence.Value).IsEqualTo(60);
            await Assert.That(team.CountryId).IsEqualTo(3);
        }
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    public async Task ShouldRejectInvalidTeamId(int id)
    {
        await Assert.That(() => new Team(
            id,
            "Test Team",
            new TeamAttributeValue(80),
            new TeamAttributeValue(70),
            new TeamAttributeValue(60),
            3)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    public async Task ShouldRejectInvalidCountryId(int countryId)
    {
        await Assert.That(() => new Team(
            7,
            "Test Team",
            new TeamAttributeValue(80),
            new TeamAttributeValue(70),
            new TeamAttributeValue(60),
            countryId)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments("")]
    [Arguments(" ")]
    public async Task ShouldRejectEmptyName(string name)
    {
        await Assert.That(() => new Team(
            7,
            name,
            new TeamAttributeValue(80),
            new TeamAttributeValue(70),
            new TeamAttributeValue(60),
            3)).Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldUpdateMutableValuesWithoutChangingIdentity()
    {
        var team = CreateTeam();

        team.Update(
            "Updated Team",
            new TeamAttributeValue(10),
            new TeamAttributeValue(20),
            new TeamAttributeValue(30));

        using (Assert.Multiple())
        {
            await Assert.That(team.Id).IsEqualTo(7);
            await Assert.That(team.CountryId).IsEqualTo(3);
            await Assert.That(team.Name).IsEqualTo("Updated Team");
            await Assert.That(team.Attack.Value).IsEqualTo(10);
            await Assert.That(team.Midfield.Value).IsEqualTo(20);
            await Assert.That(team.Defence.Value).IsEqualTo(30);
        }
    }

    [Test]
    public async Task ShouldRejectEmptyNameWhenUpdating()
    {
        var team = CreateTeam();

        await Assert.That(() => team.Update(
            "",
            new TeamAttributeValue(10),
            new TeamAttributeValue(20),
            new TeamAttributeValue(30))).Throws<ArgumentException>();
    }

    private static Team CreateTeam() => new(
        7,
        "Test Team",
        new TeamAttributeValue(80),
        new TeamAttributeValue(70),
        new TeamAttributeValue(60),
        3);
}
