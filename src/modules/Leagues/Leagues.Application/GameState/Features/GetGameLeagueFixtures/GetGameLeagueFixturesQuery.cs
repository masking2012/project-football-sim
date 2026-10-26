namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueFixtures;

public sealed record GetGameLeagueFixturesQuery(
    Guid UserId,
    Guid GameId,
    Guid SeasonId,
    int LeagueVersionId);
