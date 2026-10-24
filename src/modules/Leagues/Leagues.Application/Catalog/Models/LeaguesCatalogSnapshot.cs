namespace ProjectFootballSim.Leagues.Application.Catalog.Models;

internal sealed record LeaguesCatalogSnapshot(
    Dictionary<int, LeagueDto> LeaguesById);
