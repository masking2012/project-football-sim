namespace ProjectFootballSim.Match.Domain.ValueObjects;

public sealed record MatchSettings
{
    public bool HasHomeAdvantage { get; init; }
}
