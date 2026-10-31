namespace ProjectFootballSim.Calendar.Contracts.Features.CompleteGameSeason;

public sealed record CompleteGameSeasonCommand(
    Guid UserId,
    Guid GameId,
    Guid SeasonId);
