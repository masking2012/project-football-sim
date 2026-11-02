using ProjectFootballSim.Calendar.Domain.Enums;

namespace ProjectFootballSim.Calendar.Domain.Entities;

public sealed class GameFixture
{
    public Guid CompetitionId { get; }
    public CompetitionType CompetitionType { get; }
    public int HomeTeamId { get; }
    public int AwayTeamId { get; }
    public DateTime ScheduledAt { get; }
    public FixtureStatus Status { get; }
    public int RoundId { get; }
    public Guid UserId { get; }
    public Guid GameId { get; }

    public GameFixture(
        Guid competitionId,
        CompetitionType competitionType,
        int homeTeamId,
        int awayTeamId,
        DateTime scheduledAt,
        FixtureStatus status,
        int roundId,
        Guid userId,
        Guid gameId)
    {
        if (competitionId == Guid.Empty)
            throw new ArgumentException("Competition ID cannot be empty.", nameof(competitionId));
        if (!Enum.IsDefined(competitionType))
            throw new ArgumentException("Unsupported competition type.", nameof(competitionType));
        if (homeTeamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(homeTeamId), homeTeamId, "Home team ID must be positive.");
        if (awayTeamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(awayTeamId), awayTeamId, "Away team ID must be positive.");
        if (homeTeamId == awayTeamId)
            throw new ArgumentException("Home team and away team cannot be the same.");
        if (scheduledAt == default)
            throw new ArgumentException("Scheduled date must be specified.", nameof(scheduledAt));
        if (!Enum.IsDefined(status))
            throw new ArgumentException("Unsupported fixture status.", nameof(status));
        if (roundId <= 0)
            throw new ArgumentOutOfRangeException(nameof(roundId), roundId, "Round ID must be positive.");
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        if (gameId == Guid.Empty)
            throw new ArgumentException("Game ID cannot be empty.", nameof(gameId));

        CompetitionId = competitionId;
        CompetitionType = competitionType;
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        ScheduledAt = scheduledAt;
        Status = status;
        RoundId = roundId;
        UserId = userId;
        GameId = gameId;
    }
}
