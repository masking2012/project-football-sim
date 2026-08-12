namespace ProjectFootballSim.Leagues.Application.GameFeatures.CreateGameLeague;

public sealed record CreateGameLeagueCommand(
    Guid UserId,
    Guid GameId,
    Guid SeasonId,
    DateTime SeasonStartDate,
    Guid? PreviousSeasonId,
    int LeagueId);
