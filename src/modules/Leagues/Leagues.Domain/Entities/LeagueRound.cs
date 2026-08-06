namespace ProjectFootballSim.Leagues.Domain.Entities;

public sealed class LeagueRound
{
    public int LeagueId { get; }
    public int Round { get; }
    public int Week { get; private set; }
    public bool IsMidweek { get; private set; }

    public LeagueRound(int leagueId, int round, int week, bool isMidweek)
    {
        if (leagueId <= 0)
            throw new ArgumentException("League ID must be positive value");
        if (round <= 0)
            throw new ArgumentException("Round must be positive value");
        if (week <= 0)
            throw new ArgumentException("Week must be positive value");

        LeagueId = leagueId;
        Round = round;
        Week = week;
        IsMidweek = isMidweek;
    }

    public void Update(int week, bool isMidweek)
    {
        if (week <= 0)
            throw new ArgumentException("Week must be positive value");

        Week = week;
        IsMidweek = isMidweek;
    }
}
