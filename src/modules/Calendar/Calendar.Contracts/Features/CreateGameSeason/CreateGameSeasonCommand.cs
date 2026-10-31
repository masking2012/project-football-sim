namespace ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;

public sealed record CreateGameSeasonCommand(
    Guid UserId,
    Guid GameId);
