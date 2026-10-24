using ProjectFootballSim.Teams.Domain.ValueObjects;

namespace ProjectFootballSim.Teams.Domain.Entities;

public sealed class Team
{
    public int Id { get; }
    public string Name { get; private set; }
    public TeamAttributeValue Attack { get; private set; }
    public TeamAttributeValue Midfield { get; private set; }
    public TeamAttributeValue Defence { get; private set; }
    public int CountryId { get; }

    public Team(int id, string name, TeamAttributeValue attack, TeamAttributeValue midfield, TeamAttributeValue defence, int countryId)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Team ID must be greater than zero.");
        if (countryId <= 0)
            throw new ArgumentOutOfRangeException(nameof(countryId), "Country ID must be greater than zero.");

        ValidateName(name);

        Id = id;
        Name = name;
        Attack = attack;
        Midfield = midfield;
        Defence = defence;
        CountryId = countryId;
    }

    public void Update(string name, TeamAttributeValue attack, TeamAttributeValue midfield, TeamAttributeValue defence)
    {
        ValidateName(name);

        Name = name;
        Attack = attack;
        Midfield = midfield;
        Defence = defence;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name cannot be empty", nameof(name));
    }
}
