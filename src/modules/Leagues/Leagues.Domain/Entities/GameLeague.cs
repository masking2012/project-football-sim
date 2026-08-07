namespace ProjectFootballSim.Leagues.Domain.Entities;

public sealed class GameLeague
{
    private readonly List<GameLeagueTeam> _teams = [];
    private readonly List<GameLeagueMatch> _matches = [];

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid GameId { get; }
    public Guid SeasonId { get; }
    public int LeagueId { get; }

    public League League { get; } = default!;
    public IReadOnlyCollection<GameLeagueTeam> GameLeagueTeams => _teams;
    public IReadOnlyCollection<GameLeagueMatch> GameLeagueMatches => _matches;

    public GameLeague(Guid userId, Guid gameId, Guid seasonId, int leagueId)
    {
        UserId = userId;
        GameId = gameId;
        SeasonId = seasonId;
        LeagueId = leagueId;
    }

    public void AddGameLeagueTeam(GameLeagueTeam gameLeagueTeam)
    {
        if (_teams.Contains(gameLeagueTeam))
            throw new ArgumentException("Team is already part of the league.");

        _teams.Add(gameLeagueTeam);
    }

    public void AddGameLeagueMatch(GameLeagueMatch gameLeagueMatch)
    {
        if (gameLeagueMatch.HomeTeamId == gameLeagueMatch.AwayTeamId)
            throw new ArgumentException("Home team and away team cannot be the same.");

        if (!_teams.Select(x => x.TeamId).Contains(gameLeagueMatch.HomeTeamId) || !_teams.Select(x => x.TeamId).Contains(gameLeagueMatch.AwayTeamId))
            throw new ArgumentException("Both teams must be part of the league.");

        _matches.Add(gameLeagueMatch);
    }
}
