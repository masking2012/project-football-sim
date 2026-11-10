namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByIds;

public sealed record LeagueMatchDto(
    Guid Id,
    DateTime Date,
    int HomeTeamId,
    int AwayTeamId,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int LeagueRound,
    int LeagueVersionId);
