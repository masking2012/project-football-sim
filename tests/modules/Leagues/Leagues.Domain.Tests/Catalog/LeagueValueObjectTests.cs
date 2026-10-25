using ProjectFootballSim.Leagues.Domain.Catalog.Enums;
using ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

namespace ProjectFootballSim.Leagues.Domain.Tests.Catalog;

internal sealed class LeagueValueObjectTests
{
    [Test]
    public async Task ShouldExposeLeagueValues()
    {
        var stage = new LeagueStage(1, LeagueStageType.RoundRobin, 2);
        var league = new League(1, 2, "Premier", 3, 4, 4, [stage], [1], [2], [3], [4], null, null, null);

        using (Assert.Multiple())
        {
            await Assert.That(league.Id).IsEqualTo(1);
            await Assert.That(league.Version).IsEqualTo(2);
            await Assert.That(league.Name).IsEqualTo("Premier");
            await Assert.That(league.Format).Contains(stage);
            await Assert.That(league.PromotionPositions).Contains(1);
        }
    }

    [Test]
    public async Task ShouldRejectInvalidFormat()
    {
        await Assert.That(() => CreateLeague([])).Throws<ArgumentException>();
        await Assert.That(() => CreateLeague([new LeagueStage(2, LeagueStageType.RoundRobin, 1)]))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldRejectInvalidOrDuplicatePositions()
    {
        await Assert.That(() => CreateLeague([new LeagueStage(1, LeagueStageType.RoundRobin, 1)], [0]))
            .Throws<ArgumentException>();
        await Assert.That(() => CreateLeague([new LeagueStage(1, LeagueStageType.RoundRobin, 1)], [1], [1]))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldExposeLeagueTeamValues()
    {
        var team = new LeagueTeam(1, 2, 3);

        using (Assert.Multiple())
        {
            await Assert.That(team.LeagueId).IsEqualTo(1);
            await Assert.That(team.Version).IsEqualTo(2);
            await Assert.That(team.TeamId).IsEqualTo(3);
        }
    }

    [Test]
    [Arguments(0, 1, 1)]
    [Arguments(-1, 1, 1)]
    [Arguments(1, 0, 1)]
    [Arguments(1, -1, 1)]
    [Arguments(1, 1, 0)]
    [Arguments(1, 1, -1)]
    public async Task ShouldRejectNonPositiveLeagueTeamValues(int leagueId, int version, int teamId)
    {
        await Assert.That(() => new LeagueTeam(leagueId, version, teamId))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ShouldSupportRecordEquality()
    {
        var first = new LeagueTeam(1, 2, 3);
        var second = new LeagueTeam(1, 2, 3);
        var round = new LeagueRound(1, 2, 3, 4, true);

        using (Assert.Multiple())
        {
            await Assert.That(first).IsEqualTo(second);
            await Assert.That(first.GetHashCode()).IsEqualTo(second.GetHashCode());
            await Assert.That(round.IsMidweek).IsTrue();
        }
    }

    [Test]
    [Arguments(0, 1, 1)]
    [Arguments(1, 0, 1)]
    [Arguments(1, 1, 1)]
    public async Task ShouldRejectInvalidStageValues(int order, int legs, int type)
    {
        await Assert.That(() => new LeagueStage(order, (LeagueStageType)type, legs))
            .Throws<ArgumentException>();
    }

    [Test]
    [Arguments(0, 1, 1, 1)]
    [Arguments(1, 0, 1, 1)]
    [Arguments(1, 1, 0, 1)]
    [Arguments(1, 1, 1, 0)]
    public async Task ShouldRejectInvalidRoundValues(int leagueId, int version, int round, int week)
    {
        await Assert.That(() => new LeagueRound(leagueId, version, round, week, false))
            .Throws<ArgumentException>();
    }

    private static League CreateLeague(
        ICollection<LeagueStage> format,
        ICollection<int>? promotionPositions = null,
        ICollection<int>? promotionPlayOffPositions = null) =>
        new(1, 1, "League", 1, 1, 4, format, promotionPositions, promotionPlayOffPositions, null, null, null, null, null);
}
