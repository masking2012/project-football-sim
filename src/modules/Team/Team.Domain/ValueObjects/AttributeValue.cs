namespace ProjectFootballSim.Team.Domain.ValueObjects;

public readonly struct AttributeValue : IEquatable<AttributeValue>
{
    public int Value { get; }

    public AttributeValue(int value)
    {
        if (value < 1 || value > 99)
            throw new ArgumentOutOfRangeException(nameof(value), "Attribute value must be between 1 and 99");

        Value = value;
    }

    public override bool Equals(object obj)
    {
        if (obj is AttributeValue other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public bool Equals(AttributeValue other)
    {
        return Value.Equals(other.Value);
    }
    public static bool operator ==(AttributeValue left, AttributeValue right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(AttributeValue left, AttributeValue right)
    {
        return !(left == right);
    }
}
