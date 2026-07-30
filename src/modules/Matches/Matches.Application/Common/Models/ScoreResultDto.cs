namespace ProjectFootballSim.Matches.Application.Common.Models;

public sealed record ScoreResultDto
{
    public int HomeScore { get; }
    public int AwayScore { get; }

    public ScoreResultDto(int homeScore, int awayScore)
    {
        if (homeScore < 0)
            throw new ArgumentOutOfRangeException(nameof(homeScore), "Home score must be non-negative.");
        if (awayScore < 0)
            throw new ArgumentOutOfRangeException(nameof(awayScore), "Away score must be non-negative.");

        HomeScore = homeScore;
        AwayScore = awayScore;
    }
}
