namespace ProjectFootballSim.Api.Endpoints.Seasons;

internal sealed record CreateSeasonRequest(
    Guid GameId,
    DateTime CurrentGameDate);
