namespace ProjectFootballSim.Shared.Data.Entities.Leagues;

public sealed record LeagueRoundData
    (int LeagueId, int Version, int Round, int Week, bool IsMidweek);
