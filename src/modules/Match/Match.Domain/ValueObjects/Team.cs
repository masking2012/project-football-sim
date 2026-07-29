namespace ProjectFootballSim.Match.Domain.ValueObjects;

public sealed record Team
{
    public int Id { get; }
    public int Attack { get; }
    public int Defence { get; }
    public int Midfield { get; }

    public Team(int id, int attack, int defence, int midfield)
    {
        if (attack < 1 || attack > 100)
            throw new ArgumentOutOfRangeException(nameof(attack), "Attack must be between 1 and 100.");

        if (defence < 1 || defence > 100)
            throw new ArgumentOutOfRangeException(nameof(defence), "Defence must be between 1 and 100.");

        if (midfield < 1 || midfield > 100)
            throw new ArgumentOutOfRangeException(nameof(midfield), "Midfield must be between 1 and 100.");

        Id = id;
        Attack = attack;
        Defence = defence;
        Midfield = midfield;
    }
}
