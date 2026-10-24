namespace ProjectFootballSim.Leagues.Domain.Catalog.Entities;

public sealed class LeagueActivation
{
    public int LeagueId { get; }
    public int CurrentVersion { get; private set; }

    public LeagueActivation(int leagueId, int currentVersion)
    {
        Validate(leagueId, currentVersion);

        LeagueId = leagueId;
        CurrentVersion = currentVersion;
    }

    public void ChangeVersion(int currentVersion)
    {
        Validate(LeagueId, currentVersion);
        CurrentVersion = currentVersion;
    }

    private static void Validate(int leagueId, int currentVersion)
    {
        if (leagueId <= 0)
            throw new ArgumentOutOfRangeException(nameof(leagueId), leagueId, "League ID must be positive.");
        if (currentVersion <= 0)
            throw new ArgumentOutOfRangeException(nameof(currentVersion), currentVersion, "Current league version must be positive.");
    }
}
