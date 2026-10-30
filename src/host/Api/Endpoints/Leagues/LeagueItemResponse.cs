namespace ProjectFootballSim.Api.Endpoints.Leagues;

//TODO: think whether we need this response or we can use LeagueDto instead
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
