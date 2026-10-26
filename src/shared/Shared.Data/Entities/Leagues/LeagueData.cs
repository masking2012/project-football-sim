namespace ProjectFootballSim.Shared.Data.Entities.Leagues;

public sealed record LeagueData(
    int Id,
    string Name,
    int Order,
    int CountryId
);
