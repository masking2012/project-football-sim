using ProjectFootballSim.Leagues.Domain.Catalog.Entities;

namespace ProjectFootballSim.Leagues.Domain.Tests.Catalog;

internal sealed class LeagueActivationTests
{
    [Test]
    public async Task ShouldExposeConstructorValuesAndChangeVersion()
    {
        var activation = new LeagueActivation(7, 2);

        activation.ChangeVersion(3);

        using (Assert.Multiple())
        {
            await Assert.That(activation.LeagueId).IsEqualTo(7);
            await Assert.That(activation.LeagueVersionId).IsEqualTo(3);
        }
    }

    [Test]
    [Arguments(0, 1)]
    [Arguments(1, 0)]
    public async Task ShouldRejectNonPositiveValues(int leagueId, int version)
    {
        await Assert.That(() => new LeagueActivation(leagueId, version))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ShouldRejectInvalidVersionChanges()
    {
        var activation = new LeagueActivation(1, 1);

        await Assert.That(() => activation.ChangeVersion(0))
            .Throws<ArgumentOutOfRangeException>();
    }
}
