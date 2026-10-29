namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;

public sealed record GameLeagueMatchDto(
    Guid Id,
    int HomeTeamId,
    int AwayTeamId,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int Round,
    string LeagueName,
    int LeagueId,
    int CountryId,
    int Order);

