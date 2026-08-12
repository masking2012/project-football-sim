namespace ProjectFootballSim.Leagues.Application.Common.Models;

public sealed record LeagueDto(
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
