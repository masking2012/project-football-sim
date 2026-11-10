namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.CreateGameLeague;

public sealed record CreateGameLeagueCommand(
    Guid UserId,
    Guid GameId,
    Guid SeasonId,
    DateTime SeasonStartDate,
    Guid? PreviousSeasonId,
    int LeagueVersionId);
