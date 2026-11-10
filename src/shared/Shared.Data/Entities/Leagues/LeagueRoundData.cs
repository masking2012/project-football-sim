namespace ProjectFootballSim.Shared.Data.Entities.Leagues;

public sealed record LeagueRoundData
    (int LeagueVersionId, int Round, int Week, bool IsMidweek, IEnumerable<DayOfWeek> AvailableDaysOfWeek);

public enum DayOfWeek
{
    Su = 0,
    Mo = 1,
    Tu = 2,
    We = 3,
    Th = 4,
    Fr = 5,
    Sa = 6
}
