namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueFixtures;

public sealed record GetGameLeagueFixturesQuery(
    Guid UserId,
    Guid GameId,
    Guid SeasonId,
    int LeagueVersionId);
