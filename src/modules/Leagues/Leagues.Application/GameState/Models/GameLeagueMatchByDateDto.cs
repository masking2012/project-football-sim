using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.GameState.Models;

public sealed record GameLeagueMatchByDateDto(
    GameLeagueMatch Match,
    int LeagueVersionId);
