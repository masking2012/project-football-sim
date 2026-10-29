namespace ProjectFootballSim.Leagues.Application.GameState.Features.SimulateLeagueGameDay;

public record SimulateLeagueGameDayCommand(Guid UserId, Guid GameId, DateTime Date);
