namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.SimulateLeagueGameDay;

public record SimulateLeagueGameDayCommand(Guid UserId, Guid GameId, DateTime Date);
