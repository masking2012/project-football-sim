namespace ProjectFootballSim.Teams.Domain.ValueObjects;

public readonly struct TeamAttributeValue : IEquatable<TeamAttributeValue>
{
    public int Value { get; }

    public TeamAttributeValue(int value)
    {
        if (value < 1 || value > 99)
            throw new ArgumentOutOfRangeException(nameof(value), "Attribute value must be between 1 and 99");

        Value = value;
    }

    public override bool Equals(object obj)
    {
        if (obj is TeamAttributeValue other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public bool Equals(TeamAttributeValue other)
    {
        return Value.Equals(other.Value);
    }
    public static bool operator ==(TeamAttributeValue left, TeamAttributeValue right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(TeamAttributeValue left, TeamAttributeValue right)
    {
        return !(left == right);
    }
}
