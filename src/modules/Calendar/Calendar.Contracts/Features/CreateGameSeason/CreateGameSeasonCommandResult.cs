namespace ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;

public sealed record CreateGameSeasonCommandResult(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate);
