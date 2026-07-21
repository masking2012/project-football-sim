namespace ProjectFootballSim.ValueObjects;

public sealed record MatchResult
{
    public int HomeScore { get; }
    public int AwayScore { get; }
    public int HomeExtraTimeScore { get; }
    public int AwayExtraTimeScore { get; }
    public int HomePenaltyScore { get; }
    public int AwayPenaltyScore { get; }
    public bool ExtraTimePlayed { get; }
    public bool PenaltyShootoutPlayed { get; }

    public MatchResult(int homeScore, int awayScore)
    {
        if (homeScore < 0)
            throw new ArgumentOutOfRangeException(nameof(homeScore), "Home score must be non-negative.");
        if (awayScore < 0)
            throw new ArgumentOutOfRangeException(nameof(awayScore), "Away score must be non-negative.");

        HomeScore = homeScore;
        AwayScore = awayScore;
    }

    public MatchResult(int homeScore, int awayScore, int homeExtraTimeScore, int awayExtraTimeScore)
        : this(homeScore, awayScore)
    {
        if (homeExtraTimeScore < 0)
            throw new ArgumentOutOfRangeException(nameof(homeExtraTimeScore), "Home extra time score must be non-negative.");
        if (awayExtraTimeScore < 0)
            throw new ArgumentOutOfRangeException(nameof(awayExtraTimeScore), "Away extra time score must be non-negative.");

        HomeExtraTimeScore = homeExtraTimeScore;
        AwayExtraTimeScore = awayExtraTimeScore;
        ExtraTimePlayed = true;
    }

    //public MatchResult(int homeScore, int awayScore, int homeExtraTimeScore, int awayExtraTimeScore, int homePenaltyScore, int awayPenaltyScore)
    //{
    //    if (homeScore < 0)
    //        throw new ArgumentOutOfRangeException(nameof(homeScore), "Home score must be non-negative.");
    //    if (awayScore < 0)
    //        throw new ArgumentOutOfRangeException(nameof(awayScore), "Away score must be non-negative.");
    //    if (homeExtraTimeScore < 0)
    //        throw new ArgumentOutOfRangeException(nameof(homeExtraTimeScore), "Home extra time score must be non-negative.");
    //    if (awayExtraTimeScore < 0)
    //        throw new ArgumentOutOfRangeException(nameof(awayExtraTimeScore), "Away extra time score must be non-negative.");
    //    if (homePenaltyScore < 0)
    //        throw new ArgumentOutOfRangeException(nameof(homePenaltyScore), "Home penalty score must be non-negative.");
    //    if (awayPenaltyScore < 0)
    //        throw new ArgumentOutOfRangeException(nameof(awayPenaltyScore), "Away penalty score must be non-negative.");

    //    HomeScore = homeScore;
    //    AwayScore = awayScore;
    //    HomeExtraTimeScore = homeExtraTimeScore;
    //    AwayExtraTimeScore = awayExtraTimeScore;
    //    HomePenaltyScore = homePenaltyScore;
    //    AwayPenaltyScore = awayPenaltyScore;
    //}
}
