namespace ProjectFootballSim.Calendar.Contracts.Features.GetGameSeasons;

public record class GetGameSeasonDto(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate,
    int Order,
    bool IsCurrent);
