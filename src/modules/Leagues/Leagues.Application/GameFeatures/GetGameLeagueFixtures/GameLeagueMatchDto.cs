namespace ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueFixtures;

public sealed record GameLeagueMatchDto(
    Guid Id,
    DateTime Date,
    int HomeTeamId,
    int AwayTeamId,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int Round);
