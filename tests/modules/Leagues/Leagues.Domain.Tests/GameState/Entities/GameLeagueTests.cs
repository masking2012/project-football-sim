using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Domain.Tests.GameState.Entities;

internal sealed class GameLeagueTests
{
    [Test]
    public async Task ShouldRejectDuplicateTeam()
    {
        var gameLeague = CreateGameLeague();
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1));

        await Assert.That(() => gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1)))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldRejectChildFromAnotherGameLeague()
    {
        var gameLeague = CreateGameLeague();
        var otherGameLeague = CreateGameLeague();

        await Assert.That(() => gameLeague.AddGameLeagueTeam(new GameLeagueTeam(otherGameLeague.Id, 1)))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldRejectMatchWithTeamNotInLeague()
    {
        var gameLeague = CreateGameLeague();
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1));

        await Assert.That(() => gameLeague.AddGameLeagueMatch(new GameLeagueMatch(DateTime.UtcNow, 1, 2, 1, gameLeague.Id)))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldRejectMatchWithSameHomeAndAwayTeam()
    {
        var gameLeague = CreateGameLeague();

        await Assert.That(() => new GameLeagueMatch(DateTime.UtcNow, 1, 1, 1, gameLeague.Id))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldRejectDuplicateTeamPairInSameRound()
    {
        var gameLeague = CreateGameLeague();
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1));
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 2));
        gameLeague.AddGameLeagueMatch(new GameLeagueMatch(DateTime.UtcNow, 1, 2, 1, gameLeague.Id));

        await Assert.That(() => gameLeague.AddGameLeagueMatch(new GameLeagueMatch(DateTime.UtcNow, 2, 1, 1, gameLeague.Id)))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldRecordMatchResultAndUpdateStandings()
    {
        var gameLeague = CreateGameLeague();
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1));
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 2));
        var match = new GameLeagueMatch(DateTime.UtcNow, 1, 2, 1, gameLeague.Id);
        gameLeague.AddGameLeagueMatch(match);

        gameLeague.RecordMatchResult(match.Id, 2, 1);

        await Assert.That(match.HomeTeamScore).IsEqualTo(2);
        await Assert.That(match.AwayTeamScore).IsEqualTo(1);
        await Assert.That(gameLeague.GameLeagueTeams.Single(team => team.TeamId == 1).Wins).IsEqualTo(1);
        await Assert.That(gameLeague.GameLeagueTeams.Single(team => team.TeamId == 1).Points).IsEqualTo(3);
        await Assert.That(gameLeague.GameLeagueTeams.Single(team => team.TeamId == 2).Losses).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldRejectRecordingMatchResultTwice()
    {
        var gameLeague = CreateGameLeague();
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1));
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 2));
        var match = new GameLeagueMatch(DateTime.UtcNow, 1, 2, 1, gameLeague.Id);
        gameLeague.AddGameLeagueMatch(match);
        gameLeague.RecordMatchResult(match.Id, 0, 0);

        await Assert.That(() => gameLeague.RecordMatchResult(match.Id, 1, 0))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldRejectNegativeMatchScore()
    {
        var gameLeague = CreateGameLeagueWithMatch();

        await Assert.That(() => gameLeague.RecordMatchResult(gameLeague.GameLeagueMatches.Single().Id, -1, 0))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ShouldRecordDrawAndUpdateBothStandings()
    {
        var gameLeague = CreateGameLeagueWithMatch();
        var match = gameLeague.GameLeagueMatches.Single();

        gameLeague.RecordMatchResult(match.Id, 2, 2);

        var homeTeam = gameLeague.GameLeagueTeams.Single(team => team.TeamId == 1);
        var awayTeam = gameLeague.GameLeagueTeams.Single(team => team.TeamId == 2);
        await Assert.That(homeTeam.Draws).IsEqualTo(1);
        await Assert.That(homeTeam.Points).IsEqualTo(1);
        await Assert.That(homeTeam.GoalsFor).IsEqualTo(2);
        await Assert.That(homeTeam.GoalsAgainst).IsEqualTo(2);
        await Assert.That(awayTeam.Draws).IsEqualTo(1);
        await Assert.That(awayTeam.Points).IsEqualTo(1);
    }

    private static GameLeague CreateGameLeague() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid(),
        1);

    private static GameLeague CreateGameLeagueWithMatch()
    {
        var gameLeague = CreateGameLeague();
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1));
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 2));
        gameLeague.AddGameLeagueMatch(new GameLeagueMatch(DateTime.UtcNow, 1, 2, 1, gameLeague.Id));
        return gameLeague;
    }
}
