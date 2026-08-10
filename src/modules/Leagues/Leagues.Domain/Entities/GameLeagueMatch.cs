namespace ProjectFootballSim.Leagues.Domain.Entities;

public class GameLeagueMatch
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

        Date = date;
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        Round = round;
        GameLeagueId = gameLeagueId;
    }

    public void SetScore(int homeTeamScore, int awayTeamScore)
    {
        if (homeTeamScore < 0)
            throw new ArgumentException("Home team score cannot be negative.");
        if (awayTeamScore < 0)
            throw new ArgumentException("Away team score cannot be negative.");

        HomeTeamScore = homeTeamScore;
        AwayTeamScore = awayTeamScore;
    }
}
