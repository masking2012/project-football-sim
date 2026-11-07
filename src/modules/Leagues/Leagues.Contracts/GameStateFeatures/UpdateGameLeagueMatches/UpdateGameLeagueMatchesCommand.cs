namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.UpdateGameLeagueMatches;

public record UpdateGameLeagueMatchesCommand(
    Guid UserId,
    Guid GameId,
    IReadOnlyList<GameLeagueMatchDateReference> Matches);
