namespace ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueStandings;

public sealed record GetGameLeagueStandingsQuery(
    Guid UserId,
    Guid GameId,
    int LeagueId);
