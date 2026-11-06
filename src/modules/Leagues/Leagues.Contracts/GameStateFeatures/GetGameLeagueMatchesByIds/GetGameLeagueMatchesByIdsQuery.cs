namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByIds;

public sealed record GetGameLeagueMatchesByIdsQuery(
    Guid UserId,
    Guid GameId,
    IReadOnlyList<Guid> Ids);
