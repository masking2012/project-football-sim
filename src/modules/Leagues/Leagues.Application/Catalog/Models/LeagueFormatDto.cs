namespace ProjectFootballSim.Leagues.Application.Catalog.Models;

public sealed record LeagueFormatDto(int Order, LeagueStageTypeDto Type, int Legs);

public enum LeagueStageTypeDto
{
    RoundRobin
}
