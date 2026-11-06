using ProjectFootballSim.Leagues.Domain.GameState.Enums;

namespace ProjectFootballSim.Leagues.Domain.GameState.Entities;

public sealed class GameLeagueRound
{
    public Guid GameLeagueId { get; }
    public int Round { get; }
    public DateTime BaseDate { get; }
    public GameLeagueRoundStatus RoundStatus { get; private set; }

    public GameLeagueRound(Guid gameLeagueId, int round, DateTime baseDate)
    {
        if (gameLeagueId == Guid.Empty)
            throw new ArgumentException("Game league ID cannot be empty.", nameof(gameLeagueId));
        if (round <= 0)
            throw new ArgumentOutOfRangeException(nameof(round), round, "Round must be positive.");
        if (baseDate == default)
            throw new ArgumentException("Round base date must be specified.", nameof(baseDate));

        GameLeagueId = gameLeagueId;
        Round = round;
        BaseDate = baseDate;
        RoundStatus = GameLeagueRoundStatus.Provisional;
    }

    public void UpdateStatus(GameLeagueRoundStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status), status, "Round status is not defined.");

        RoundStatus = status;
    }
}
