using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Domain.Tests.GameState.Entities;

internal sealed class GameLeagueTeamTests
{
    [Test]
    public async Task ShouldUpdateStatisticsForWinLossAndDraw()
    {
        var gameLeague = CreateGameLeagueWithMatches();
        var matches = gameLeague.GameLeagueMatches.OrderBy(match => match.Round).ToArray();
        gameLeague.RecordMatchResult(matches[0].Id, 3, 1);
        gameLeague.RecordMatchResult(matches[1].Id, 0, 2);
        gameLeague.RecordMatchResult(matches[2].Id, 1, 1);
        var team = gameLeague.GameLeagueTeams.Single(team => team.TeamId == 1);

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
        var gameLeague = CreateGameLeagueWithMatches();
        var match = gameLeague.GameLeagueMatches.First();

        await Assert.That(() => gameLeague.RecordMatchResult(match.Id, goalsFor, goalsAgainst))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ShouldRejectInvalidIdentity()
    {
        await Assert.That(() => new GameLeagueTeam(Guid.Empty, 1)).Throws<ArgumentException>();
        await Assert.That(() => new GameLeagueTeam(Guid.NewGuid(), 0)).Throws<ArgumentOutOfRangeException>();
    }

    private static GameLeague CreateGameLeagueWithMatches()
    {
        var gameLeague = new GameLeague(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1);
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1));
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 2));
        for (var round = 1; round <= 3; round++)
            gameLeague.AddGameLeagueMatch(new GameLeagueMatch(DateTime.UtcNow, 1, 2, round, gameLeague.Id));
        return gameLeague;
    }
}
