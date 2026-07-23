namespace ProjectFootballSim.Match.Domain.ValueObjects;

public sealed record ScoreResult
{
    public int HomeScore { get; }
    public int AwayScore { get; }

    public ScoreResult(int homeScore, int awayScore)
    {
        if (homeScore < 0)
            throw new ArgumentOutOfRangeException(nameof(homeScore), "Home score must be non-negative.");
        if (awayScore < 0)
            throw new ArgumentOutOfRangeException(nameof(awayScore), "Away score must be non-negative.");

        HomeScore = homeScore;
        AwayScore = awayScore;
    }
}
