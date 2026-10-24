namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueStandings;

public sealed record GetGameLeagueStandingsQuery(
    Guid UserId,
    Guid GameId,
    Guid SeasonId,
    int LeagueId);
