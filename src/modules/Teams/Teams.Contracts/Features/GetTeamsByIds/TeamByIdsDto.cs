namespace ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;

public sealed record TeamByIdsDto(
    int Id,
    string Name,
    int Attack,
    int Midfield,
    int Defence,
    int CountryId);
