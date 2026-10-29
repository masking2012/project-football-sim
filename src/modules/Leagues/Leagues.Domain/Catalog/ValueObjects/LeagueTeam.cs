namespace ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

public sealed record LeagueTeam
{
    public int LeagueVersionId { get; }
    public int TeamId { get; }

    public LeagueTeam(int leagueVersionId, int teamId)
    {
        if (leagueVersionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(leagueVersionId), leagueVersionId, "League version ID must be positive.");
        if (teamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(teamId), teamId, "Team ID must be positive.");

        LeagueVersionId = leagueVersionId;
        TeamId = teamId;
    }
}
