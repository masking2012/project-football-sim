namespace ProjectFootballSim.Leagues.Domain.Entities;

public sealed class GameLeague
{
    private readonly List<GameLeagueTeam> _items = [];

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid GameId { get; }
    public Guid SeasonId { get; }
    public int LeagueId { get; }

    public League League { get; } = default!;
    public IReadOnlyCollection<GameLeagueTeam> GameLeagueTeams => _items;

    public GameLeague(Guid userId, Guid gameId, Guid seasonId, int leagueId)
    {
        UserId = userId;
        GameId = gameId;
        SeasonId = seasonId;
        LeagueId = leagueId;
    }

    public void AddGameLeagueTeam(GameLeagueTeam gameLeagueTeam)
    {
        _items.Add(gameLeagueTeam);
    }
}
