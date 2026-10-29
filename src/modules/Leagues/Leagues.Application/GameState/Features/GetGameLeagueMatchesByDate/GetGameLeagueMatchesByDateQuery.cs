namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;

public sealed record GetGameLeagueMatchesByDateQuery(
    Guid UserId,
    Guid GameId,
    DateTime Date);
