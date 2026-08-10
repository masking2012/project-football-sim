namespace ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;

internal sealed record MatchEventResponse(
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
