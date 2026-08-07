namespace ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

public sealed record GameLeagueMatchDto(
    Guid Id,
    int HomeTeamId,
    int AwayTeamId,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int Round,
    string LeagueName,
    int LeagueId,
    int CountryId);

