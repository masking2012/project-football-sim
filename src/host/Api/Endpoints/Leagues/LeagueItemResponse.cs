namespace ProjectFootballSim.Api.Endpoints.Leagues;

internal sealed record LeagueItemResponse(
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
    ICollection<int>? UefaConferenceLeaguePositions);
