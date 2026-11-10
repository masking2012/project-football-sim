namespace ProjectFootballSim.Calendar.Contracts.Features.ProvisionGameFixtures;

public sealed record ProvisionGameFixturesCommand(
    Guid UserId,
    Guid GameId,
    Guid GameLeagueId,
    int LeagueVersionId,
    IReadOnlyList<ProvisionGameFixtureItem> Fixtures);

public sealed record ProvisionGameFixtureItem(
    Guid MatchId,
    int HomeTeamId,
    int AwayTeamId,
    DateTime ScheduledAt,
    int RoundId);
