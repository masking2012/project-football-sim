namespace ProjectFootballSim.Matches.Domain.ValueObjects;

public readonly struct AdvantageRatio : IEquatable<AdvantageRatio>
{
    public static readonly AdvantageRatio Neutral = new AdvantageRatio(1.00);

    public double Value { get; }

    public AdvantageRatio(double value)
    {
        if (value < 1.00 || value > 2.00)
            throw new ArgumentOutOfRangeException(nameof(value), "Advantage ratio must be between 1.00 and 2.00");

        Value = value;
    }

    public override bool Equals(object obj)
    {
        if (obj is AdvantageRatio other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public bool Equals(AdvantageRatio other)
    {
        return Value.Equals(other.Value);
    }
    public static bool operator ==(AdvantageRatio left, AdvantageRatio right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(AdvantageRatio left, AdvantageRatio right)
    {
        return !(left == right);
    }
}
