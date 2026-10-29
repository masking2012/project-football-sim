namespace ProjectFootballSim.Shared.Data.Entities.Leagues;

public sealed record LeagueFormatData(int Order, LeagueFormatType Type, int Legs);

public enum LeagueFormatType
{
    RoundRobin
}
