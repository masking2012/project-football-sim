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

    public GameLeagueMatch(DateTime date, int homeTeamId, int awayTeamId, int round, Guid gameLeagueId)
    {
        if (date == default)
            throw new ArgumentException("Match date must be specified.", nameof(date));
        if (homeTeamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(homeTeamId), homeTeamId, "Home team ID must be positive.");
        if (awayTeamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(awayTeamId), awayTeamId, "Away team ID must be positive.");
        if (homeTeamId == awayTeamId)
            throw new ArgumentException("Home team and away team cannot be the same.");
        if (round <= 0)
            throw new ArgumentOutOfRangeException(nameof(round), round, "Round must be positive.");
        if (gameLeagueId == Guid.Empty)
            throw new ArgumentException("Game league ID cannot be empty.", nameof(gameLeagueId));

        Id = Guid.NewGuid();
        Date = date;
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        Round = round;
        GameLeagueId = gameLeagueId;
    }

    internal void SetScore(int homeTeamScore, int awayTeamScore)
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
