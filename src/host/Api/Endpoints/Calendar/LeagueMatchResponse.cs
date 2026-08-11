namespace ProjectFootballSim.Api.Endpoints.Calendar;

internal sealed record LeagueMatchResponse(
    Guid Id,
    int HomeTeamId,
    string HomeTeamName,
    int AwayTeamId,
    string AwayTeamName,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int Round,
    string LeagueName,
    int LeagueId,
    int CountryId,
    string CountryName);
