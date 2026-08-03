namespace ProjectFootballSim.Leagues.Domain.Entities;

public sealed class League
{
    public int Id { get; }
    public string Name { get; private set; }
    public int Order { get; private set; }
    public int CountryId { get; private set; }

    public League(int id, string name, int order, int countryId)
    {
        if (id <= 0)
            throw new ArgumentException("League ID must be positive value");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("League name cannot be null or empty");
        if (order <= 0)
            throw new ArgumentException("League order must be positive value");
        if (countryId <= 0)
            throw new ArgumentException("Country ID must be positive value");

        Id = id;
        Name = name;
        Order = order;
        CountryId = countryId;
    }

    public void Update(string name, int order, int countryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("League name cannot be null or empty");
        if (order <= 0)
            throw new ArgumentException("League order must be positive value");
        if (countryId <= 0)
            throw new ArgumentException("Country ID must be positive value");

        Name = name;
        Order = order;
        CountryId = countryId;
    }
}
