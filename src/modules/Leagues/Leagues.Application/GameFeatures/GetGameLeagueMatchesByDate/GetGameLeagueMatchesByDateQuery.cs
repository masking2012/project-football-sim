namespace ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

public sealed record GetGameLeagueMatchesByDateQuery(
    Guid UserId,
    Guid GameId,
    DateTime Date);
