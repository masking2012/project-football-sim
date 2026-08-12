namespace ProjectFootballSim.Calendar.Application.Features.GetGameSeasons;

public record class GameSeasonDto(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate,
    int Order,
    bool IsCurrent);
