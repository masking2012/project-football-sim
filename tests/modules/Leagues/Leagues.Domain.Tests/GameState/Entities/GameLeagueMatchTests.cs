using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Domain.Tests.GameState.Entities;

internal sealed class GameLeagueMatchTests
{
    [Test]
    public async Task ShouldExposeConstructorValuesAndSetScore()
    {
        var date = new DateTime(2026, 10, 26, 12, 30, 0, DateTimeKind.Utc);
        var gameLeague = CreateGameLeagueWithMatch(round: 3, date: date);
        var match = gameLeague.GameLeagueMatches.Single();

        gameLeague.RecordMatchResult(match.Id, 2, 0);

        using (Assert.Multiple())
        {
            await Assert.That(match.Date).IsEqualTo(date);
            await Assert.That(match.HomeTeamId).IsEqualTo(1);
            await Assert.That(match.AwayTeamId).IsEqualTo(2);
            await Assert.That(match.Round).IsEqualTo(3);
            await Assert.That(match.GameLeagueId).IsEqualTo(gameLeague.Id);
            await Assert.That(match.HomeTeamScore).IsEqualTo(2);
            await Assert.That(match.AwayTeamScore).IsEqualTo(0);
        }
    }

    [Test]
    public async Task ShouldRejectInvalidMatchValues()
    {
        await Assert.That(() => new GameLeagueMatch(DateTime.UtcNow, 0, 2, 1, Guid.NewGuid())).Throws<ArgumentException>();
        await Assert.That(() => new GameLeagueMatch(DateTime.UtcNow, 1, 0, 1, Guid.NewGuid())).Throws<ArgumentException>();
        await Assert.That(() => new GameLeagueMatch(DateTime.UtcNow, 1, 1, 1, Guid.NewGuid())).Throws<ArgumentException>();
        await Assert.That(() => new GameLeagueMatch(DateTime.UtcNow, 1, 2, 0, Guid.NewGuid())).Throws<ArgumentException>();
        await Assert.That(() => new GameLeagueMatch(DateTime.UtcNow, 1, 2, 1, Guid.Empty)).Throws<ArgumentException>();
        await Assert.That(() => new GameLeagueMatch(default, 1, 2, 1, Guid.NewGuid())).Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldRejectInvalidOrRepeatedScores()
    {
        var gameLeague = CreateGameLeagueWithMatch();
        var match = gameLeague.GameLeagueMatches.Single();

        await Assert.That(() => gameLeague.RecordMatchResult(match.Id, -1, 0)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => gameLeague.RecordMatchResult(match.Id, 0, -1)).Throws<ArgumentOutOfRangeException>();
        gameLeague.RecordMatchResult(match.Id, 0, 0);
        await Assert.That(() => gameLeague.RecordMatchResult(match.Id, 1, 0)).Throws<InvalidOperationException>();
    }

    private static GameLeague CreateGameLeagueWithMatch(int round = 1, DateTime? date = null)
    {
        var gameLeague = new GameLeague(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1);
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1));
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 2));
        gameLeague.AddGameLeagueMatch(new GameLeagueMatch(date ?? DateTime.UtcNow, 1, 2, round, gameLeague.Id));
        return gameLeague;
    }
}
