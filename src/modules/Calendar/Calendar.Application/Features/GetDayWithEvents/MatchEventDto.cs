namespace ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;

public sealed record MatchEventDto(
    Guid Id,
    int HomeTeamId,
    int AwayTeamId,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int Round,
    string LeagueName,
    int LeagueId,
    int CountryId);
