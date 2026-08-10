namespace ProjectFootballSim.Seasons.Application.Features.CreatePlayerSeason;

public sealed record CreateGameSeasonCommand(
    Guid GameId,
    Guid UserId);
