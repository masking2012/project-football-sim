using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Domain.Tests.GameState.Entities;

internal sealed class GameLeagueMatchTests
{
    [Test]
    public async Task ShouldExposeConstructorValuesAndSetScore()
    {
        var gameLeagueId = Guid.NewGuid();
        var date = DateTime.UtcNow;
        var match = new GameLeagueMatch(date, 1, 2, 3, gameLeagueId);

        match.SetScore(2, 0);

        using (Assert.Multiple())
        {
            await Assert.That(match.Date).IsEqualTo(date);
            await Assert.That(match.HomeTeamId).IsEqualTo(1);
            await Assert.That(match.AwayTeamId).IsEqualTo(2);
            await Assert.That(match.Round).IsEqualTo(3);
            await Assert.That(match.GameLeagueId).IsEqualTo(gameLeagueId);
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
    }

    [Test]
    public async Task ShouldRejectInvalidOrRepeatedScores()
    {
        var match = new GameLeagueMatch(DateTime.UtcNow, 1, 2, 1, Guid.NewGuid());

        await Assert.That(() => match.SetScore(-1, 0)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => match.SetScore(0, -1)).Throws<ArgumentOutOfRangeException>();
        match.SetScore(0, 0);
        await Assert.That(() => match.SetScore(1, 0)).Throws<InvalidOperationException>();
    }
}
