namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueFixtures;

public sealed record LeagueMatchDto(
    Guid Id,
    DateTime Date,
    int HomeTeamId,
    int AwayTeamId,
    string HomeTeamName,
    string AwayTeamName,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int Round);
