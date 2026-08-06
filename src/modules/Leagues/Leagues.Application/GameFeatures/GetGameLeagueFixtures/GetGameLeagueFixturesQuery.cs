namespace ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueFixtures;

public sealed record GetGameLeagueFixturesQuery(
    Guid GameId,
    Guid SeasonId,
    int LeagueId);
