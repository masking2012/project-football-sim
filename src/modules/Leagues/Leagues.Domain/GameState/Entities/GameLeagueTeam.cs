namespace ProjectFootballSim.Leagues.Domain.GameState.Entities;

public sealed class GameLeagueTeam
{
    public Guid GameLeagueId { get; }
    public int TeamId { get; }
    public int Wins { get; private set; }
    public int Draws { get; private set; }
    public int Losses { get; private set; }
    public int GoalsFor { get; private set; }
    public int GoalsAgainst { get; private set; }
    public int Points { get; private set; }

    public GameLeagueTeam(Guid gameLeagueId, int teamId)
    {
        if (gameLeagueId == Guid.Empty)
            throw new ArgumentException("Game league ID cannot be empty.", nameof(gameLeagueId));
        if (teamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(teamId), teamId, "Team ID must be positive.");

        GameLeagueId = gameLeagueId;
        TeamId = teamId;
    }

    public void AddWin(int goalsFor, int goalsAgainst)
    {
        ValidateGoals(goalsFor, goalsAgainst);
        Wins++;
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
        Points += 3;
    }

    public void AddLoss(int goalsFor, int goalsAgainst)
    {
        ValidateGoals(goalsFor, goalsAgainst);
        Losses++;
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
    }

    public void AddDraw(int goalsFor, int goalsAgainst)
    {
        ValidateGoals(goalsFor, goalsAgainst);
        Draws++;
        GoalsFor += goalsFor;
        GoalsAgainst += goalsAgainst;
        Points += 1;
    }

    private static void ValidateGoals(int goalsFor, int goalsAgainst)
    {
        if (goalsFor < 0)
            throw new ArgumentOutOfRangeException(nameof(goalsFor), goalsFor, "Goals for cannot be negative.");
        if (goalsAgainst < 0)
            throw new ArgumentOutOfRangeException(nameof(goalsAgainst), goalsAgainst, "Goals against cannot be negative.");
    }
}
