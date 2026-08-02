namespace ProjectFootballSim.Seasons.Application.Features.CreateNextPlayerSeason;

public sealed record CreatePlayerSeasonCommand(Guid GameId, Guid UserId, DateTime CurrentDate);
