namespace ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueStandings;

public sealed record GameLeagueTeamDto(
    int TeamId,
    int Wins,
    int Draws,
    int Losses,
    int GoalsFor,
    int GoalsAgainst,
    int Points,
    int Position);
