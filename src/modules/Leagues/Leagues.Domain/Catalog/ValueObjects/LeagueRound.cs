namespace ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

public sealed record LeagueRound
{
    public int LeagueId { get; }
    public int Version { get; }
    public int Round { get; }
    public int Week { get; }
    public bool IsMidweek { get; }

    public LeagueRound(int leagueId, int version, int round, int week, bool isMidweek)
    {
        if (leagueId <= 0)
            throw new ArgumentException("League ID must be positive value");
        if (round <= 0)
            throw new ArgumentException("Round must be positive value");
        if (version <= 0)
            throw new ArgumentException("League version must be positive value");
        if (week <= 0)
            throw new ArgumentException("Week must be positive value");

        LeagueId = leagueId;
        Version = version;
        Round = round;
        Week = week;
        IsMidweek = isMidweek;
    }
}
