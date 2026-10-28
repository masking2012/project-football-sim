namespace ProjectFootballSim.Teams.Contracts.Features.GetTeamsByCountry;

public sealed record TeamByCountryDto(
    int Id,
    string Name,
    int Attack,
    int Midfield,
    int Defence);
