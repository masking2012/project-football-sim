using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Domain.Tests.GameState.Entities;

internal sealed class GameLeagueTeamTests
{
    [Test]
    public async Task ShouldUpdateStatisticsForWinLossAndDraw()
    {
        var team = new GameLeagueTeam(Guid.NewGuid(), 1);

        team.AddWin(3, 1);
        team.AddLoss(0, 2);
        team.AddDraw(1, 1);

        using (Assert.Multiple())
        {
            await Assert.That(team.Wins).IsEqualTo(1);
            await Assert.That(team.Losses).IsEqualTo(1);
            await Assert.That(team.Draws).IsEqualTo(1);
            await Assert.That(team.GoalsFor).IsEqualTo(4);
            await Assert.That(team.GoalsAgainst).IsEqualTo(4);
            await Assert.That(team.Points).IsEqualTo(4);
        }
    }

    [Test]
    [Arguments(-1, 0)]
    [Arguments(0, -1)]
    public async Task ShouldRejectNegativeGoals(int goalsFor, int goalsAgainst)
    {
        var team = new GameLeagueTeam(Guid.NewGuid(), 1);

        await Assert.That(() => team.AddWin(goalsFor, goalsAgainst))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ShouldRejectInvalidIdentity()
    {
        await Assert.That(() => new GameLeagueTeam(Guid.Empty, 1)).Throws<ArgumentException>();
        await Assert.That(() => new GameLeagueTeam(Guid.NewGuid(), 0)).Throws<ArgumentOutOfRangeException>();
    }
}
