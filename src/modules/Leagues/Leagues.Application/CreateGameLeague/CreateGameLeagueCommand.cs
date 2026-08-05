namespace ProjectFootballSim.Leagues.Application.CreateGameLeague;

public sealed record CreateGameLeagueCommand(
    Guid UserId,
    Guid GameId,
    Guid SeasonId,
    int LeagueId);
