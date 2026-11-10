namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueRoundsToSchedule;

public sealed record ProvisionalRoundDto(
    Guid GameLeagueId,
    int LeagueVersionId,
    int Round,
    DateTime BaseDate);
