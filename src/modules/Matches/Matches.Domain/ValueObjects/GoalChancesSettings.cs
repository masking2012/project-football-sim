namespace ProjectFootballSim.Matches.Domain.ValueObjects;

public readonly struct GoalChancesSettings : IEquatable<GoalChancesSettings>
{
    public static GoalChancesSettings RegularTime => new GoalChancesSettings(0, 22, 20);
    public static GoalChancesSettings ExtraTime => new GoalChancesSettings(0, 7, 6);

    public int Minimum { get; }
    public int Maximum { get; }
    public int BaseNumber { get; }

    private GoalChancesSettings(int minimum, int maximum, int baseNumber)
    {
        if (minimum < 0)
            throw new ArgumentOutOfRangeException(nameof(minimum), "Minimum goal chances must be non-negative.");
        if (maximum < minimum)
            throw new ArgumentOutOfRangeException(nameof(maximum), "Maximum goal chances must be greater than or equal to minimum.");
        if (baseNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(baseNumber), "Base number of goal chances must be non-negative.");

        Minimum = minimum;
        Maximum = maximum;
        BaseNumber = baseNumber;
    }

    public override bool Equals(object obj)
    {
        if (obj is GoalChancesSettings other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Minimum, Maximum, BaseNumber);
    }

    public bool Equals(GoalChancesSettings other)
    {
        return (Minimum == other.Minimum && Maximum == other.Maximum && BaseNumber == other.BaseNumber);
    }
    public static bool operator ==(GoalChancesSettings left, GoalChancesSettings right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(GoalChancesSettings left, GoalChancesSettings right)
    {
        return !(left == right);
    }
}
