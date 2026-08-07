namespace ProjectFootballSim.Api.Endpoints.Leagues;

internal sealed record MatchItemResponse(
    Guid Id,
    DateTime Date,
    int HomeTeamId,
    int AwayTeamId,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int Round,
    string HomeTeamName,
    string AwayTeamName);
