namespace ProjectFootballSim.Api.Endpoints.Leagues;

internal sealed record TeamStandingItemResponse(
    int TeamId,
    string Name,
    int Wins,
    int Draws,
    int Losses,
    int GoalsFor,
    int GoalsAgainst,
    int Points
);
