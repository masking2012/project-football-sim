namespace ProjectFootballSim.Leagues.Domain.GameState.Entities;

public sealed class GameLeagueMatch
{
    public Guid Id { get; }
    public DateTime Date { get; }
    public int HomeTeamId { get; }
    public int AwayTeamId { get; }
    public int? HomeTeamScore { get; private set; }
    public int? AwayTeamScore { get; private set; }
    public int Round { get; }
    public Guid GameLeagueId { get; }

    public GameLeague GameLeague { get; } = default!;

    public GameLeagueMatch(DateTime date, int homeTeamId, int awayTeamId, int round, Guid gameLeagueId)
    {
        if (homeTeamId <= 0)
            throw new ArgumentException("Home team ID must be positive value");
        if (awayTeamId <= 0)
            throw new ArgumentException("Away team ID must be positive value");
        if (round <= 0)
            throw new ArgumentException("Round must be positive value");
        if (gameLeagueId == Guid.Empty)
            throw new ArgumentException("Game league ID cannot be empty.", nameof(gameLeagueId));

        Id = Guid.NewGuid();
        Date = date;
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        Round = round;
        GameLeagueId = gameLeagueId;
    }

    public void SetScore(int homeTeamScore, int awayTeamScore)
    {
        if (HomeTeamScore.HasValue || AwayTeamScore.HasValue)
            throw new InvalidOperationException("A result has already been recorded for this match.");
        if (homeTeamScore < 0)
            throw new ArgumentOutOfRangeException(nameof(homeTeamScore), homeTeamScore, "Home team score cannot be negative.");
        if (awayTeamScore < 0)
            throw new ArgumentOutOfRangeException(nameof(awayTeamScore), awayTeamScore, "Away team score cannot be negative.");

        HomeTeamScore = homeTeamScore;
        AwayTeamScore = awayTeamScore;
    }
}
