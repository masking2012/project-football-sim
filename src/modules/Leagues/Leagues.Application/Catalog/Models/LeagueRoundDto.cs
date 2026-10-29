namespace ProjectFootballSim.Leagues.Application.Catalog.Models;

public sealed record LeagueRoundDto(int LeagueId, int LeagueVersionId, int Round, int Week, bool IsMidweek);
