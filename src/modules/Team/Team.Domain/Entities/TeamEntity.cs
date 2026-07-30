using ProjectFootballSim.Team.Domain.ValueObjects;

namespace ProjectFootballSim.Team.Domain.Entities;

public sealed class TeamEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public AttributeValue Attack { get; private set; }
    public AttributeValue Midfield { get; private set; }
    public AttributeValue Defence { get; private set; }
    public int CountryId { get; private set; }

    public TeamEntity(int id, string name, AttributeValue attack, AttributeValue midfield, AttributeValue defence, int countryId)
    {
        ValidateName(name);

        Id = id;
        Name = name;
        Attack = attack;
        Midfield = midfield;
        Defence = defence;
        CountryId = countryId;
    }

    public void Update(string name, AttributeValue attack, AttributeValue midfield, AttributeValue defence, int countryId)
    {
        ValidateName(name);

        Name = name;
        Attack = attack;
        Midfield = midfield;
        Defence = defence;
        CountryId = countryId;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name cannot be empty", nameof(name));
    }
}
