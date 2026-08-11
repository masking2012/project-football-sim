namespace ProjectFootballSim.Common.Data.Entities.Leagues;

public sealed record LeagueData(
    int Id,
    string Name,
    int Order,
    int CountryId,
    int TeamsCount,
    ICollection<int>? PromotionPositions,
    ICollection<int>? PromotionPlayOffPositions,
    ICollection<int>? RelegationPositions,
    ICollection<int>? RelegationPlayOffPositions,
    ICollection<int>? UefaChampionsLeaguePositions,
    ICollection<int>? UefaEuropaLeaguePositions,
    ICollection<int>? UefaConferenceLeaguePositions
);
