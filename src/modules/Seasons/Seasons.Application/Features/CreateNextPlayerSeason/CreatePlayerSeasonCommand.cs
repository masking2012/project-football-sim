namespace ProjectFootballSim.Seasons.Application.Features.CreateNextPlayerSeason;

public sealed record CreatePlayerSeasonCommand(Guid UserId, DateTime CurrentDate);
