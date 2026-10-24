namespace ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

public sealed class LeagueFormat
{
    public int Order { get; }
    public LeagueFormatType Type { get; }
    public int Legs { get; }

    public LeagueFormat(int order, LeagueFormatType type, int legs)
    {
        if (order <= 0)
            throw new ArgumentException("League format order must be positive.", nameof(order));
        if (legs <= 0)
            throw new ArgumentException("League format legs must be positive.", nameof(legs));
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported league format type.");

        Order = order;
        Type = type;
        Legs = legs;
    }
}

public enum LeagueFormatType
{
    RoundRobin
}
