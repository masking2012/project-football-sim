namespace ProjectFootballSim.Leagues.Domain.GameState.Entities;

public sealed class GameLeague
{
    private readonly List<GameLeagueTeam> _teams = [];
    private readonly List<GameLeagueMatch> _matches = [];

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid GameId { get; }
    public Guid SeasonId { get; }
    public int LeagueId { get; }

    public IReadOnlyCollection<GameLeagueTeam> GameLeagueTeams => _teams;
    public IReadOnlyCollection<GameLeagueMatch> GameLeagueMatches => _matches;

    public GameLeague(Guid userId, Guid gameId, Guid seasonId, int leagueId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        if (gameId == Guid.Empty)
            throw new ArgumentException("Game ID cannot be empty.", nameof(gameId));
        if (seasonId == Guid.Empty)
            throw new ArgumentException("Season ID cannot be empty.", nameof(seasonId));
        if (leagueId <= 0)
            throw new ArgumentOutOfRangeException(nameof(leagueId), leagueId, "League ID must be positive.");

        Id = Guid.NewGuid();
        UserId = userId;
        GameId = gameId;
        SeasonId = seasonId;
        LeagueId = leagueId;
    }

    public void AddGameLeagueTeam(GameLeagueTeam gameLeagueTeam)
    {
        ArgumentNullException.ThrowIfNull(gameLeagueTeam);

        if (gameLeagueTeam.GameLeagueId != Id)
            throw new ArgumentException("Team belongs to a different game league.", nameof(gameLeagueTeam));
        if (_teams.Any(team => team.TeamId == gameLeagueTeam.TeamId))
            throw new InvalidOperationException("Team is already part of the league.");

        _teams.Add(gameLeagueTeam);
    }

    public void AddGameLeagueMatch(GameLeagueMatch gameLeagueMatch)
    {
        ArgumentNullException.ThrowIfNull(gameLeagueMatch);

        if (gameLeagueMatch.GameLeagueId != Id)
            throw new ArgumentException("Match belongs to a different game league.", nameof(gameLeagueMatch));
        if (gameLeagueMatch.HomeTeamId == gameLeagueMatch.AwayTeamId)
            throw new ArgumentException("Home team and away team cannot be the same.");
        if (!ContainsTeam(gameLeagueMatch.HomeTeamId) || !ContainsTeam(gameLeagueMatch.AwayTeamId))
            throw new ArgumentException("Both teams must be part of the league.");
        if (_matches.Any(match => match.Id == gameLeagueMatch.Id))
            throw new InvalidOperationException("Match is already part of the league.");
        if (_matches.Any(match =>
            match.Round == gameLeagueMatch.Round
            && ((match.HomeTeamId == gameLeagueMatch.HomeTeamId && match.AwayTeamId == gameLeagueMatch.AwayTeamId)
                || (match.HomeTeamId == gameLeagueMatch.AwayTeamId && match.AwayTeamId == gameLeagueMatch.HomeTeamId))))
            throw new InvalidOperationException("A match between these teams already exists in this round.");

        _matches.Add(gameLeagueMatch);
    }

    public void RecordMatchResult(Guid matchId, int homeTeamScore, int awayTeamScore)
    {
        var match = _matches.SingleOrDefault(candidate => candidate.Id == matchId)
            ?? throw new InvalidOperationException($"Match with ID '{matchId}' is not part of the league.");

        match.SetScore(homeTeamScore, awayTeamScore);

        var homeTeam = GetTeam(match.HomeTeamId);
        var awayTeam = GetTeam(match.AwayTeamId);

        if (homeTeamScore > awayTeamScore)
        {
            homeTeam.AddWin(homeTeamScore, awayTeamScore);
            awayTeam.AddLoss(awayTeamScore, homeTeamScore);
        }
        else if (homeTeamScore < awayTeamScore)
        {
            homeTeam.AddLoss(homeTeamScore, awayTeamScore);
            awayTeam.AddWin(awayTeamScore, homeTeamScore);
        }
        else
        {
            homeTeam.AddDraw(homeTeamScore, awayTeamScore);
            awayTeam.AddDraw(awayTeamScore, homeTeamScore);
        }
    }

    private bool ContainsTeam(int teamId) => _teams.Any(team => team.TeamId == teamId);

    private GameLeagueTeam GetTeam(int teamId) => _teams.SingleOrDefault(team => team.TeamId == teamId)
        ?? throw new InvalidOperationException($"Team with ID '{teamId}' is not part of the league.");
}
