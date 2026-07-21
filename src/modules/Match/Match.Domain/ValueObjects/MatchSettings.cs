namespace ProjectFootballSim.ValueObjects;

public sealed record MatchSettings
{
    public bool HasHomeAdvantage { get; init; }
    public bool HasExtraTime { get; init; }
    public bool HasPenaltyShootout { get; init; }
}
