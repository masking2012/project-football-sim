namespace ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;

public sealed record CreateGameSeasonCommand(
    Guid UserId,
    Guid GameId);
