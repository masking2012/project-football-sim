namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.UpdateGameLeagueRoundStatus;

public record UpdateGameLeagueRoundStatusCommand(
    Guid UserId,
    Guid GameId,
    IReadOnlyList<GameLeagueRoundReference> Rounds,
    GameLeagueRoundStatusContract Status);
