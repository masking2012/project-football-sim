namespace ProjectFootballSim.Calendar.Contracts;

public sealed record GameSeasonStartedEvent(
    Guid UserId,
    Guid GameId,
    Guid SeasonId,
    DateTime StartDate,
    Guid? PreviousSeasonId);
