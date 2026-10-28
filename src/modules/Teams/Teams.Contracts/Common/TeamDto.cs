namespace ProjectFootballSim.Teams.Contracts.Common;

public sealed record TeamDto(
    int Id,
    string Name,
    int Attack,
    int Midfield,
    int Defence,
    int CountryId);
