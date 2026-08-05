namespace ProjectFootballSim.Leagues.Domain.Entities;

public sealed class GameLeague
{
    public Guid Id { get; }
    public Guid GameId { get; }
    public int LeagueId { get; }
    public Guid UserId { get; }
    public League League { get; } = default!;

    public ICollection<GameLeagueTeam> GameLeagueTeams { get; } = new List<GameLeagueTeam>();

    public GameLeague(Guid gameId, int leagueId, Guid userId)
    {
        GameId = gameId;
        LeagueId = leagueId;
        UserId = userId;
    }
}
