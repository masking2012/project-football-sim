namespace ProjectFootballSim.Leagues.Application.GameFeatures.SimulateLeagueGameDay;

public record SimulateLeagueGameDayCommand(Guid UserId, Guid GameId, DateTime Date);
