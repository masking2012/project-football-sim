using ProjectFootballSim.Leagues.Domain.Catalog.Enums;
using ProjectFootballSim.Leagues.Domain.Catalog.Entities;
using ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

namespace ProjectFootballSim.Leagues.Domain.Tests.Catalog;

internal sealed class LeagueValueObjectTests
{
    [Test]
    public async Task ShouldExposeLeagueValues()
    {
        var league = new League(1, "Premier", 3, 4);

        using (Assert.Multiple())
        {
            await Assert.That(league.Id).IsEqualTo(1);
            await Assert.That(league.Name).IsEqualTo("Premier");
            await Assert.That(league.Order).IsEqualTo(3);
            await Assert.That(league.CountryId).IsEqualTo(4);
        }
    }

    [Test]
    public async Task ShouldExposeLeagueVersionValues()
    {
        var stage = new LeagueStage(1, LeagueStageType.RoundRobin, 2);
        var version = CreateLeagueVersion([stage], [1]);

        using (Assert.Multiple())
        {
            await Assert.That(version.LeagueVersionId).IsEqualTo(101001);
            await Assert.That(version.LeagueId).IsEqualTo(1);
            await Assert.That(version.Version).IsEqualTo(1);
            await Assert.That(version.Format).Contains(stage);
            await Assert.That(version.PromotionPositions).Contains(1);
        }
    }

    [Test]
    public async Task ShouldRejectInvalidFormat()
    {
        await Assert.That(() => CreateLeagueVersion([])).Throws<ArgumentException>();
        await Assert.That(() => CreateLeagueVersion([new LeagueStage(2, LeagueStageType.RoundRobin, 1)]))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldRejectInvalidOrDuplicatePositions()
    {
        await Assert.That(() => CreateLeagueVersion([new LeagueStage(1, LeagueStageType.RoundRobin, 1)], [0]))
            .Throws<ArgumentException>();
        await Assert.That(() => CreateLeagueVersion([new LeagueStage(1, LeagueStageType.RoundRobin, 1)], [1], [1]))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldExposeLeagueTeamValues()
    {
        var team = new LeagueTeam(101001, 3);

        using (Assert.Multiple())
        {
            await Assert.That(team.LeagueVersionId).IsEqualTo(101001);
            await Assert.That(team.TeamId).IsEqualTo(3);
        }
    }

    [Test]
    public async Task ShouldSupportRecordEquality()
    {
        var first = new LeagueTeam(101001, 3);
        var second = new LeagueTeam(101001, 3);
        var round = new LeagueRound(101001, 3, 4, true);

        using (Assert.Multiple())
        {
            await Assert.That(first).IsEqualTo(second);
            await Assert.That(first.GetHashCode()).IsEqualTo(second.GetHashCode());
            await Assert.That(round.IsMidweek).IsTrue();
        }
    }

    [Test]
    [Arguments(0, 1)]
    [Arguments(-1, 1)]
    [Arguments(1, 0)]
    [Arguments(1, -1)]
    public async Task ShouldRejectInvalidLeagueTeamValues(int leagueVersionId, int teamId)
    {
        await Assert.That(() => new LeagueTeam(leagueVersionId, teamId))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(0, 1, 1)]
    [Arguments(1, 0, 1)]
    [Arguments(1, 1, 0)]
    public async Task ShouldRejectInvalidRoundValues(int leagueVersionId, int round, int week)
    {
        await Assert.That(() => new LeagueRound(leagueVersionId, round, week, false))
            .Throws<ArgumentException>();
    }

    private static LeagueVersion CreateLeagueVersion(
        ICollection<LeagueStage> format,
        ICollection<int>? promotionPositions = null,
        ICollection<int>? promotionPlayOffPositions = null) =>
        new(101001, 1, 1, 4, format, promotionPositions, promotionPlayOffPositions, null, null, null, null, null);
}
