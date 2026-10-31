namespace ProjectFootballSim.Calendar.Application.Common.Models;

public record class GameSeasonDto(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate);
