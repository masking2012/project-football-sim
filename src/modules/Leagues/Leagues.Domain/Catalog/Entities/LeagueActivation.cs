namespace ProjectFootballSim.Leagues.Domain.Catalog.Entities;

public sealed class LeagueActivation
{
    public int LeagueId { get; }
    public int LeagueVersionId { get; private set; }

    public LeagueActivation(int leagueId, int leagueVersionId)
    {
        Validate(leagueId, leagueVersionId);

        LeagueId = leagueId;
        LeagueVersionId = leagueVersionId;
    }

    public void ChangeVersion(int leagueVersionId)
    {
        Validate(LeagueId, leagueVersionId);
        LeagueVersionId = leagueVersionId;
    }

    private static void Validate(int leagueId, int leagueVersionId)
    {
        if (leagueId <= 0)
            throw new ArgumentOutOfRangeException(nameof(leagueId), leagueId, "League ID must be positive.");
        if (leagueVersionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(leagueVersionId), leagueVersionId, "League version ID must be positive.");
    }
}
