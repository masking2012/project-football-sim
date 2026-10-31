namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.CheckGameLeagueMatchesByDate;

public sealed record CheckGameLeagueMatchesByDateQuery(
    Guid UserId,
    Guid GameId,
    DateTime Date);
