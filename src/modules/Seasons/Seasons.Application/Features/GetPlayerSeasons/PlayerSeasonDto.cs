namespace ProjectFootballSim.Seasons.Application.Features.GetPlayerSeasons;

public record class PlayerSeasonDto(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate,
    bool IsCurrent);
