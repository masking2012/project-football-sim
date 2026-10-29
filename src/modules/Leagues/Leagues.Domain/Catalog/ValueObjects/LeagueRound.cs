namespace ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

public sealed record LeagueRound
{
    public int LeagueVersionId { get; }
    public int Round { get; }
    public int Week { get; }
    public bool IsMidweek { get; }

    public LeagueRound(int leagueVersionId, int round, int week, bool isMidweek)
    {
        if (leagueVersionId <= 0)
            throw new ArgumentException("League version ID must be positive value");
        if (round <= 0)
            throw new ArgumentException("Round must be positive value");
        if (week <= 0)
            throw new ArgumentException("Week must be positive value");

        LeagueVersionId = leagueVersionId;
        Round = round;
        Week = week;
        IsMidweek = isMidweek;
    }
}
