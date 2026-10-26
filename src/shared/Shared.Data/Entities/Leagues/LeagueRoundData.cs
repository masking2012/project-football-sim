namespace ProjectFootballSim.Shared.Data.Entities.Leagues;

public sealed record LeagueRoundData
    (int LeagueVersionId, int Round, int Week, bool IsMidweek);
