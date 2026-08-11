namespace ProjectFootballSim.Seasons.Application.Features.CreateGameSeason;

public sealed record CreateGameSeasonCommand(
    Guid UserId,
    Guid GameId);
