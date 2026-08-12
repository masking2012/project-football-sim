namespace ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;

public sealed record LeagueMatchDto(
    Guid Id,
    int HomeTeamId,
    int AwayTeamId,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int Round,
    string LeagueName,
    int LeagueId,
    int CountryId);
