namespace ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;

public sealed record CreateGameSeasonCommandResult(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate);
