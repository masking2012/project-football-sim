namespace ProjectFootballSim.Calendar.Application.Features.CompleteGameSeason;

public sealed record CompleteGameSeasonCommandResult(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate);
