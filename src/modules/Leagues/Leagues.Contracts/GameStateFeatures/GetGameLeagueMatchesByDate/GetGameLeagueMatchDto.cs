namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByDate;

public sealed record GetGameLeagueMatchDto(
    Guid Id,
    int HomeTeamId,
    int AwayTeamId,
    string HomeTeamName,
    string AwayTeamName,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int LeagueVersionId,
    string LeagueName,
    int LeagueRound,
    int LeagueOrder,
    int CountryId,
    string CountryName);
