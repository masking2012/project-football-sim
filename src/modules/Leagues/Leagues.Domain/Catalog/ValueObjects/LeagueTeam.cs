namespace ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

public sealed record LeagueTeam
{
    public int LeagueId { get; }
    public int Version { get; }
    public int TeamId { get; }

    public LeagueTeam(int LeagueId, int Version, int TeamId)
    {
        if (LeagueId <= 0)
            throw new ArgumentOutOfRangeException(nameof(LeagueId), LeagueId, "League ID must be positive.");
        if (Version <= 0)
            throw new ArgumentOutOfRangeException(nameof(Version), Version, "League version must be positive.");
        if (TeamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(TeamId), TeamId, "Team ID must be positive.");

        this.LeagueId = LeagueId;
        this.Version = Version;
        this.TeamId = TeamId;
    }
}
