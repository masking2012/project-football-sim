namespace ProjectFootballSim.Seasons.Application.Features.CreatePlayerSeason;

public sealed record CreatePlayerSeasonCommand(
    Guid GameId,
    Guid UserId,
    DateTime CurrentGameDate);
