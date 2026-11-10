namespace ProjectFootballSim.Leagues.Contracts.GameStateFeatures.Events;

public sealed record GameLeagueMatchesProvisionalEvent(
    Guid UserId,
    Guid GameId,
    int LeagueVersionId,
    Guid GameLeagueId,
    IReadOnlyList<EventLeagueMatch> Matches);

public sealed record EventLeagueMatch(
    Guid MatchId,
    int HomeTeamId,
    int AwayTeamId,
    DateTime ScheduledAt,
    int Round
);
