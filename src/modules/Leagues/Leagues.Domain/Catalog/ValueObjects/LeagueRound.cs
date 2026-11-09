namespace ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

public sealed record LeagueRound
{
    private static readonly IEqualityComparer<HashSet<DayOfWeek>> AvailableDaysComparer =
        HashSet<DayOfWeek>.CreateSetComparer();

    public int LeagueVersionId { get; }
    public int Round { get; }
    public int Week { get; }
    public bool IsMidweek { get; }
    public HashSet<DayOfWeek> AvailableDaysOfWeek { get; }

    public LeagueRound(int leagueVersionId, int round, int week, bool isMidweek, HashSet<DayOfWeek> availableDaysOfWeek)
    {
        if (leagueVersionId <= 0)
            throw new ArgumentException("League version ID must be positive value");
        if (round <= 0)
            throw new ArgumentException("Round must be positive value");
        if (week <= 0)
            throw new ArgumentException("Week must be positive value");
        ArgumentNullException.ThrowIfNull(availableDaysOfWeek);

        LeagueVersionId = leagueVersionId;
        Round = round;
        Week = week;
        IsMidweek = isMidweek;
        AvailableDaysOfWeek = availableDaysOfWeek;
    }

    public bool Equals(LeagueRound? other) =>
        other is not null
        && LeagueVersionId == other.LeagueVersionId
        && Round == other.Round
        && Week == other.Week
        && IsMidweek == other.IsMidweek
        && AvailableDaysComparer.Equals(AvailableDaysOfWeek, other.AvailableDaysOfWeek);

    public override int GetHashCode() =>
        HashCode.Combine(
            LeagueVersionId,
            Round,
            Week,
            IsMidweek,
            AvailableDaysComparer.GetHashCode(AvailableDaysOfWeek));
}
