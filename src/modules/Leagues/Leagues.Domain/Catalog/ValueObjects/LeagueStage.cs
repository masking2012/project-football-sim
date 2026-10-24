using ProjectFootballSim.Leagues.Domain.Catalog.Enums;

namespace ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

public sealed class LeagueStage
{
    public int Order { get; }
    public LeagueStageType Type { get; }
    public int Legs { get; }

    public LeagueStage(int order, LeagueStageType type, int legs)
    {
        if (order <= 0)
            throw new ArgumentException("League stage order must be positive.", nameof(order));
        if (legs <= 0)
            throw new ArgumentException("League stage legs must be positive.", nameof(legs));
        if (!Enum.IsDefined(type))
            throw new ArgumentException("Unsupported league stage type.", nameof(type));

        Order = order;
        Type = type;
        Legs = legs;
    }
}
