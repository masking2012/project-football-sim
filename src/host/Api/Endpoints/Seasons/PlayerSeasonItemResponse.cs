namespace ProjectFootballSim.Api.Endpoints.Seasons;

internal sealed record PlayerSeasonItemResponse(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate,
    bool IsCurrent);
