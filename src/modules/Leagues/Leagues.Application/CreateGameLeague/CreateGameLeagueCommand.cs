namespace ProjectFootballSim.Leagues.Application.CreateGameLeague;

public sealed record CreateGameLeagueCommand(int LeagueId, Guid GameId, Guid UserId);
