namespace ProjectFootballSim.Shared.Data.Entities.Seasons;

public sealed record SeasonDefinitionData(
    int FirstSeasonYear,
    int StartSeasonMonth,
    int StartSeasonDay,
    int EndSeasonMonth,
    int EndSeasonDay);
