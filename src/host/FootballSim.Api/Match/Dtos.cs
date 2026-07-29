namespace ProjectFootballSim.Api.Match;

internal sealed record SimulateMatchRequest(int HomeTeamId, int AwayTeamId, bool HasHomeAdvantage);

internal sealed record TeamDto(int Id, string Name, int Attack, int Defence, int Midfield);

internal sealed record ScoreDto(int HomeScore, int AwayScore);

internal sealed record MatchResultResponse(
    TeamDto HomeTeam,
    TeamDto AwayTeam,
    ScoreDto RegularTime,
    ScoreDto? ExtraTime,
    ScoreDto? Penalties,
    ScoreDto FinalScore,
    string Winner
);
