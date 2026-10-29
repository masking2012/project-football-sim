namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagues;

public sealed record GetGameLeaguesQuery(
    Guid UserId,
    Guid GameId);
