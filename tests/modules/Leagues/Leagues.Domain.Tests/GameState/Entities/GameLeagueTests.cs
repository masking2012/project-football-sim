using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Domain.Tests.GameState.Entities;

internal sealed class GameLeagueTests
{
    [Test]
    public async Task ShouldExposeConstructorValues()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var gameLeague = new GameLeague(userId, gameId, seasonId, 2);

        using (Assert.Multiple())
        {
            await Assert.That(gameLeague.UserId).IsEqualTo(userId);
            await Assert.That(gameLeague.GameId).IsEqualTo(gameId);
            await Assert.That(gameLeague.SeasonId).IsEqualTo(seasonId);
            await Assert.That(gameLeague.LeagueVersionId).IsEqualTo(2);
            await Assert.That(gameLeague.GameLeagueTeams).IsEmpty();
            await Assert.That(gameLeague.GameLeagueMatches).IsEmpty();
        }
    }

    [Test]
    [Arguments(0, 1, 1, 1)]
    [Arguments(1, 0, 1, 1)]
    [Arguments(1, 1, 0, 1)]
    [Arguments(1, 1, 1, 0)]
    public async Task ShouldRejectInvalidConstructorValues(int userId, int gameId, int seasonId, int leagueVersionId)
    {
        await Assert.That(() => new GameLeague(
            userId == 0 ? Guid.Empty : Guid.NewGuid(),
            gameId == 0 ? Guid.Empty : Guid.NewGuid(),
            seasonId == 0 ? Guid.Empty : Guid.NewGuid(),
            leagueVersionId)).Throws<ArgumentException>();
    }

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
    public async Task ShouldRejectNullTeam()
    {
        var gameLeague = CreateGameLeague();

        await Assert.That(() => gameLeague.AddGameLeagueTeam(null!))
            .Throws<ArgumentNullException>();
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
    public async Task ShouldRejectMatchFromAnotherLeague()
    {
        var gameLeague = CreateGameLeagueWithTeams();
        var otherGameLeague = CreateGameLeague();

        await Assert.That(() => gameLeague.AddGameLeagueMatch(
            new GameLeagueMatch(DateTime.UtcNow, 1, 2, 1, otherGameLeague.Id)))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldRejectDuplicateMatchInstance()
    {
        var gameLeague = CreateGameLeagueWithMatch();
        var match = gameLeague.GameLeagueMatches.Single();

        await Assert.That(() => gameLeague.AddGameLeagueMatch(match))
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
        var match = gameLeague.GameLeagueMatches.Single();

        await Assert.That(() => gameLeague.RecordMatchResult(match.Id, -1, 0))
            .Throws<ArgumentOutOfRangeException>();

        var homeTeam = gameLeague.GameLeagueTeams.Single(team => team.TeamId == 1);
        var awayTeam = gameLeague.GameLeagueTeams.Single(team => team.TeamId == 2);
        using (Assert.Multiple())
        {
            await Assert.That(match.HomeTeamScore).IsNull();
            await Assert.That(match.AwayTeamScore).IsNull();
            await Assert.That(homeTeam.Wins).IsEqualTo(0);
            await Assert.That(homeTeam.Draws).IsEqualTo(0);
            await Assert.That(homeTeam.Losses).IsEqualTo(0);
            await Assert.That(awayTeam.Wins).IsEqualTo(0);
            await Assert.That(awayTeam.Draws).IsEqualTo(0);
            await Assert.That(awayTeam.Losses).IsEqualTo(0);
        }
    }

    [Test]
    public async Task ShouldAllowSameTeamPairInDifferentRounds()
    {
        var gameLeague = CreateGameLeagueWithTeams();

        gameLeague.AddGameLeagueMatch(new GameLeagueMatch(DateTime.UtcNow, 1, 2, 1, gameLeague.Id));
        gameLeague.AddGameLeagueMatch(new GameLeagueMatch(DateTime.UtcNow, 2, 1, 2, gameLeague.Id));

        await Assert.That(gameLeague.GameLeagueMatches.Count).IsEqualTo(2);
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

    [Test]
    public async Task ShouldRecordAwayWinAndUpdateBothStandings()
    {
        var gameLeague = CreateGameLeagueWithMatch();
        var match = gameLeague.GameLeagueMatches.Single();

        gameLeague.RecordMatchResult(match.Id, 1, 2);

        await Assert.That(gameLeague.GameLeagueTeams.Single(team => team.TeamId == 1).Losses).IsEqualTo(1);
        await Assert.That(gameLeague.GameLeagueTeams.Single(team => team.TeamId == 2).Wins).IsEqualTo(1);
        await Assert.That(gameLeague.GameLeagueTeams.Single(team => team.TeamId == 2).Points).IsEqualTo(3);
    }

    [Test]
    public async Task ShouldRejectUnknownMatchResult()
    {
        var gameLeague = CreateGameLeague();

        await Assert.That(() => gameLeague.RecordMatchResult(Guid.NewGuid(), 0, 0))
            .Throws<InvalidOperationException>();
    }

    private static GameLeague CreateGameLeague() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid(),
        1);

    private static GameLeague CreateGameLeagueWithMatch()
    {
        var gameLeague = CreateGameLeagueWithTeams();
        gameLeague.AddGameLeagueMatch(new GameLeagueMatch(DateTime.UtcNow, 1, 2, 1, gameLeague.Id));
        return gameLeague;
    }

    private static GameLeague CreateGameLeagueWithTeams()
    {
        var gameLeague = CreateGameLeague();
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1));
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 2));
        return gameLeague;
    }
}
