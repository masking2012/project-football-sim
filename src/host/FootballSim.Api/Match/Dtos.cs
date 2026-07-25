namespace ProjectFootballSim.Api.Match;

internal sealed record SimulateMatchRequest(Guid HomeTeamId, Guid AwayTeamId, bool HasHomeAdvantage);

internal sealed record TeamDto(Guid Id, string Name, int Attack, int Defence, int Midfield);

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
