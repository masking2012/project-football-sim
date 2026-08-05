namespace ProjectFootballSim.Leagues.Domain.Entities;

public sealed class GameLeague
{
    private readonly List<GameLeagueTeam> _items = [];

    public Guid Id { get; }
    public Guid GameId { get; }
    public int LeagueId { get; }
    public Guid UserId { get; }
    public League League { get; } = default!;

    public IReadOnlyCollection<GameLeagueTeam> GameLeagueTeams => _items;

    public GameLeague(Guid gameId, int leagueId, Guid userId)
    {
        GameId = gameId;
        LeagueId = leagueId;
        UserId = userId;
    }

    public void AddGameLeagueTeam(GameLeagueTeam gameLeagueTeam)
    {
        _items.Add(gameLeagueTeam);
    }
}
