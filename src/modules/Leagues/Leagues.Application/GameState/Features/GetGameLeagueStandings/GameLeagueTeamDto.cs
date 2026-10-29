namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueStandings;

public sealed record GameLeagueTeamDto(
    int TeamId,
    int Wins,
    int Draws,
    int Losses,
    int GoalsFor,
    int GoalsAgainst,
    int Points);
