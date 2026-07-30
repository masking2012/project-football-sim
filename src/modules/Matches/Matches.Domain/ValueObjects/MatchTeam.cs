namespace ProjectFootballSim.Matches.Domain.ValueObjects;

public sealed record MatchTeam
{
    public int Id { get; }
    public int Attack { get; }
    public int Defence { get; }
    public int Midfield { get; }

    public MatchTeam(int id, int attack, int defence, int midfield)
    {
        if (attack < 1 || attack > 99)
            throw new ArgumentOutOfRangeException(nameof(attack), "Attack must be between 1 and 99.");

        if (defence < 1 || defence > 99)
            throw new ArgumentOutOfRangeException(nameof(defence), "Defence must be between 1 and 99.");

        if (midfield < 1 || midfield > 99)
            throw new ArgumentOutOfRangeException(nameof(midfield), "Midfield must be between 1 and 99.");

        Id = id;
        Attack = attack;
        Defence = defence;
        Midfield = midfield;
    }
}
