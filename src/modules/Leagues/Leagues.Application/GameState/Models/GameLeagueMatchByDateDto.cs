using ProjectFootballSim.Leagues.Domain.GameState.Entities;

namespace ProjectFootballSim.Leagues.Application.Persistence.Models;

public sealed record GameLeagueMatchByDate(
    GameLeagueMatch Match,
    int LeagueVersionId);
