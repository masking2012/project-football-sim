namespace ProjectFootballSim.Api.Endpoints.Seasons;

internal sealed record CurrentSeasonResponse(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate);
