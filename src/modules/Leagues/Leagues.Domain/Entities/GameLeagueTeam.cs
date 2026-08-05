namespace ProjectFootballSim.Leagues.Domain.Entities;

public sealed class GameLeagueTeam
{
    public Guid Id { get; }
    public Guid GameLeagueId { get; }
    public int TeamId { get; }
    public int Wins { get; private set; }
    public int Draws { get; private set; }
    public int Losses { get; private set; }
    public int GoalsFor { get; private set; }
    public int GoalsAgainst { get; private set; }
    public int Points { get; private set; }

    public GameLeague GameLeague { get; } = default!;

    public GameLeagueTeam(Guid gameLeagueId, int teamId)
    {
        GameLeagueId = gameLeagueId;
        TeamId = teamId;
    }
}
