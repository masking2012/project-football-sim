namespace ProjectFootballSim.Teams.Application.Common.Models;

internal sealed record TeamsCatalogSnapshot(
    IReadOnlyDictionary<int, TeamDto> TeamsById,
    IReadOnlyDictionary<int, int[]> TeamIdsByCountry);
