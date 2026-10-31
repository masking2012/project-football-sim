namespace ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;

public sealed record GameSeasonStartedEvent(
    Guid UserId,
    Guid GameId,
    Guid SeasonId,
    DateTime StartDate,
    Guid? PreviousSeasonId);
