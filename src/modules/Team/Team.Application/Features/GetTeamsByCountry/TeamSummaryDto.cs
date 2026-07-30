namespace ProjectFootballSim.Team.Application.Features.GetTeamsByCountry;

public sealed class TeamSummaryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int Attack { get; init; }
    public required int Midfield { get; init; }
    public required int Defence { get; init; }
}
