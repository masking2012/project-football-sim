namespace ProjectFootballSim.Leagues.Application.GetGameLeagueStandings;

public sealed record GameLeagueTeamDto(
    int TeamId,
    int Wins,
    int Draws,
    int Losses,
    int GoalsFor,
    int GoalsAgainst,
    int Points);
