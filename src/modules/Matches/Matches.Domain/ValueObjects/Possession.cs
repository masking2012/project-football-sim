namespace ProjectFootballSim.Matches.Domain.ValueObjects;

public readonly struct Possession : IEquatable<Possession>
{
    public double Value { get; }
    public Possession OpponentPossession => new Possession(1.00 - Value);

    public Possession(double value)
    {
        if (value < 0.00 || value > 1.00)
            throw new ArgumentOutOfRangeException(nameof(value), "Possession must be between 0.00 and 1.00");

        Value = value;
    }

    public override bool Equals(object obj)
    {
        if (obj is Possession other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public bool Equals(Possession other)
    {
        return Value.Equals(other.Value);
    }

    public static bool operator ==(Possession left, Possession right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Possession left, Possession right)
    {
        return !(left == right);
    }
}
