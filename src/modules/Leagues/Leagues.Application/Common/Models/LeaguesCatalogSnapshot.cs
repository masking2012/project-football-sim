namespace ProjectFootballSim.Leagues.Application.Common.Models;

internal sealed record LeaguesCatalogSnapshot(
    Dictionary<int, LeagueDto> LeaguesById);
