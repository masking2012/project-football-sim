namespace ProjectFootballSim.Common.Data.Entities.Leagues;

public sealed record LeagueRoundData
    (int LeagueId, int Round, int Week, bool IsMidweek);
