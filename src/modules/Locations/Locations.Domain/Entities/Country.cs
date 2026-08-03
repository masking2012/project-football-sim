namespace ProjectFootballSim.Locations.Domain.Entities;

public sealed class Country
{
    public int Id { get; }
    public string Name { get; }
    public string Code { get; }

    public Country(int id, string name, string code)
    {
        if (id <= 0)
            throw new ArgumentException("Country ID must be positive value");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Country name cannot be null or empty");

        if (string.IsNullOrWhiteSpace(code) || code.Length != 3 || !code.All(char.IsUpper))
            throw new ArgumentException("Country code must be a 3-letter uppercase string");

        Id = id;
        Name = name;
        Code = code;
    }
}
