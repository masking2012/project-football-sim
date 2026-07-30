namespace ProjectFootballSim.Api.Endpoints.Match;

internal sealed record SimulateMatchRequest(string HomeTeamId, string AwayTeamId, bool HasHomeAdvantage);

internal sealed record TeamResponse(string Id, string Name, int Attack, int Defence, int Midfield);

internal sealed record ScoreResponse(int HomeScore, int AwayScore);

internal sealed record MatchResultResponse(
    TeamResponse HomeTeam,
    TeamResponse AwayTeam,
    ScoreResponse RegularTime,
    ScoreResponse? ExtraTime,
    ScoreResponse? Penalties,
    ScoreResponse FinalScore,
    string Winner
);
