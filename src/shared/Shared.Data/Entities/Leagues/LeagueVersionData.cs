namespace ProjectFootballSim.Shared.Data.Entities.Leagues;

public sealed record LeagueVersionData(
    int LeagueVersionId,
    int LeagueId,
    int Version,
    int TeamsCount,
    ICollection<int>? PromotionPositions,
    ICollection<int>? PromotionPlayOffPositions,
    ICollection<int>? RelegationPositions,
    ICollection<int>? RelegationPlayOffPositions,
    ICollection<int>? UefaChampionsLeaguePositions,
    ICollection<int>? UefaEuropaLeaguePositions,
    ICollection<int>? UefaConferenceLeaguePositions,
    IReadOnlyList<LeagueFormatData> Format
);
