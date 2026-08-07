namespace ProjectFootballSim.Teams.Application.Common.Models;

internal sealed record TeamsCatalogSnapshot(
    Dictionary<int, TeamDto> TeamsById,
    Dictionary<int, int[]> TeamIdsByCountry);
