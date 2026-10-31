namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByDate;

public sealed record GetGameLeagueMatchesByDateQuery(
    Guid UserId,
    Guid GameId,
    DateTime Date);
