namespace ProjectFootballSim.Api.Endpoints.Seasons;

internal sealed record GameSeasonItemResponse(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate,
    bool IsCurrent);
