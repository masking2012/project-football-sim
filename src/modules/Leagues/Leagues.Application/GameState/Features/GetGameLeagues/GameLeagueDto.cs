using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagues;

public sealed record GameLeagueDto(
    Guid GameLeagueId,
    Guid SeasonId,
    LeagueDto League);
