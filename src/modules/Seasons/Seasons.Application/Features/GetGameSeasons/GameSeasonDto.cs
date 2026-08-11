namespace ProjectFootballSim.Seasons.Application.Features.GetPlayerSeasons;

public record class GameSeasonDto(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate,
    int Order,
    bool IsCurrent);
