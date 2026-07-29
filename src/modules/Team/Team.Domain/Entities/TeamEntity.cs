using ProjectFootballSim.Team.Domain.ValueObjects;

namespace ProjectFootballSim.Team.Domain.Entities;

public sealed class TeamEntity
{
    public int Id { get; }
    public string Name { get; }
    public AttributeValue Attack { get; }
    public AttributeValue Midfield { get; }
    public AttributeValue Defence { get; }
    public int CountryId { get; }

    public TeamEntity(int id, string name, AttributeValue attack, AttributeValue midfield, AttributeValue defence, int countryId)
    {
        Id = id;
        Name = name;
        Attack = attack;
        Midfield = midfield;
        Defence = defence;
        CountryId = countryId;
    }
}
