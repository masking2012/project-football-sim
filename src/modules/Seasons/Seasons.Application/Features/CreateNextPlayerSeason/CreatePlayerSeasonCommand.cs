namespace ProjectFootballSim.Seasons.Domain.Entities;

public sealed record CreatePlayerSeasonCommand(Guid UserId, DateTime CurrentDate);
