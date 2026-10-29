namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueStandings;

public sealed record LeagueStandingsTeamDto(
    int TeamId,
    string Name,
    int Wins,
    int Draws,
    int Losses,
    int GoalsFor,
    int GoalsAgainst,
    int Points,
    int Position);
