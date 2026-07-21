namespace ProjectFootballSim.Match.Domain.ValueObjects;

public sealed record Team
{
    public Guid Id { get; }
    public int Attack { get; }
    public int Defence { get; }
    public int Midfield { get; }

    public Team(Guid id, int attack, int defense, int midfield)
    {
        if (attack < 0 || attack > 100)
            throw new ArgumentOutOfRangeException(nameof(attack), "Attack must be between 0 and 100.");

        if (defense < 0 || defense > 100)
            throw new ArgumentOutOfRangeException(nameof(defense), "Defense must be between 0 and 100.");

        if (midfield < 0 || midfield > 100)
            throw new ArgumentOutOfRangeException(nameof(midfield), "Midfield must be between 0 and 100.");

        Id = id;
        Attack = attack;
        Defence = defense;
        Midfield = midfield;
    }
}
