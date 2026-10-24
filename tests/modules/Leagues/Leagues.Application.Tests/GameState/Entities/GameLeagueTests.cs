using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.Tests.GameState.Entities;

internal sealed class GameLeagueTests
{
    [Test]
    public async Task ShouldRejectDuplicateTeamAsync()
    {
        var gameLeague = CreateGameLeague();
        gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1));

        await Assert.That(() => gameLeague.AddGameLeagueTeam(new GameLeagueTeam(gameLeague.Id, 1)))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldRejectChildFromAnotherGameLeagueAsync()
    {
        var gameLeague = CreateGameLeague();
        var otherGameLeague = CreateGameLeague();

        await Assert.That(() => gameLeague.AddGameLeagueTeam(new GameLeagueTeam(otherGameLeague.Id, 1)))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ShouldRecordMatchResultAndUpdateStandingsAsync()
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
    public async Task ShouldRejectRecordingMatchResultTwiceAsync()
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

    private static GameLeague CreateGameLeague() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid(),
        1);
}
