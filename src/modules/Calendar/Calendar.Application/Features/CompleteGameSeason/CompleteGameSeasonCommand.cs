namespace ProjectFootballSim.Calendar.Application.Features.CompleteGameSeason;

public sealed record CompleteGameSeasonCommand(
    Guid UserId,
    Guid GameId,
    Guid SeasonId);
