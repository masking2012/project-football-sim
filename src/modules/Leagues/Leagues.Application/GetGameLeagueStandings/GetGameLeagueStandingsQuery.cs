namespace ProjectFootballSim.Leagues.Application.GetGameLeagueStandings;

public sealed record GetGameLeagueStandingsQuery(
    Guid UserId,
    Guid GameId,
    int LeagueId);
