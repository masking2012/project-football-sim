namespace ProjectFootballSim.Leagues.Application.GameState.Features.CreateGameLeague;

public sealed record CreateGameLeagueCommand(
    Guid UserId,
    Guid GameId,
    Guid SeasonId,
    DateTime SeasonStartDate,
    Guid? PreviousSeasonId,
    int LeagueVersionId);
