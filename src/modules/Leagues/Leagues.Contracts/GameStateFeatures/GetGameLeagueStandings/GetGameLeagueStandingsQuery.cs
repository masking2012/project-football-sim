namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueStandings;

public sealed record GetGameLeagueStandingsQuery(
    Guid UserId,
    Guid GameId,
    Guid SeasonId,
    int LeagueVersionId);
