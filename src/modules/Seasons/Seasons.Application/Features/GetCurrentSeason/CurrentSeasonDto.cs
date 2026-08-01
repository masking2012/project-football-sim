namespace ProjectFootballSim.Seasons.Application.Features.GetCurrentSeason;

public record class CurrentSeasonDto(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate);
