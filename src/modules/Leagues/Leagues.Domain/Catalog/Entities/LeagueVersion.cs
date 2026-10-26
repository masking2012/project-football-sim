using ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

namespace ProjectFootballSim.Leagues.Domain.Catalog.Entities;

public sealed class LeagueVersion
{
    public int LeagueVersionId { get; }
    public int LeagueId { get; }
    public int Version { get; }
    public int TeamsCount { get; }
    public ICollection<LeagueStage> Format { get; }
    public ICollection<int>? PromotionPositions { get; }
    public ICollection<int>? PromotionPlayOffPositions { get; }
    public ICollection<int>? RelegationPositions { get; }
    public ICollection<int>? RelegationPlayOffPositions { get; }
    public ICollection<int>? UefaChampionsLeaguePositions { get; }
    public ICollection<int>? UefaEuropaLeaguePositions { get; }
    public ICollection<int>? UefaConferenceLeaguePositions { get; }

    public LeagueVersion(
        int leagueVersionId,
        int leagueId,
        int version,
        int teamsCount,
        ICollection<LeagueStage> format,
        ICollection<int>? promotionPositions,
        ICollection<int>? promotionPlayOffPositions,
        ICollection<int>? relegationPositions,
        ICollection<int>? relegationPlayOffPositions,
        ICollection<int>? uefaChampionsLeaguePositions,
        ICollection<int>? uefaEuropaLeaguePositions,
        ICollection<int>? uefaConferenceLeaguePositions)
    {
        if (leagueVersionId <= 0)
            throw new ArgumentException("League version ID must be positive value");
        if (leagueId <= 0)
            throw new ArgumentException("League ID must be positive value");
        if (version <= 0)
            throw new ArgumentException("League version must be positive.");
        if (teamsCount < 2)
            throw new ArgumentException("Teams count in league shouldn't be less than 2");

        ValidateFormat(format);
        ValidatePositions(
            teamsCount,
            promotionPositions,
            promotionPlayOffPositions,
            relegationPositions,
            relegationPlayOffPositions,
            uefaChampionsLeaguePositions,
            uefaEuropaLeaguePositions,
            uefaConferenceLeaguePositions);

        LeagueVersionId = leagueVersionId;
        LeagueId = leagueId;
        Version = version;
        TeamsCount = teamsCount;
        Format = format;
        PromotionPositions = promotionPositions;
        PromotionPlayOffPositions = promotionPlayOffPositions;
        RelegationPositions = relegationPositions;
        RelegationPlayOffPositions = relegationPlayOffPositions;
        UefaChampionsLeaguePositions = uefaChampionsLeaguePositions;
        UefaEuropaLeaguePositions = uefaEuropaLeaguePositions;
        UefaConferenceLeaguePositions = uefaConferenceLeaguePositions;
    }

    private static void ValidateFormat(ICollection<LeagueStage> stages)
    {
        ArgumentNullException.ThrowIfNull(stages);
        if (stages.Count == 0)
            throw new ArgumentException("League format must contain at least one stage.", nameof(stages));

        var orderedStages = stages.OrderBy(stage => stage.Order).ToList();
        for (var index = 0; index < orderedStages.Count; index++)
        {
            if (orderedStages[index].Order != index + 1)
                throw new ArgumentException("League stage orders must be unique and contiguous starting at one.", nameof(stages));
        }
    }

    private static void ValidatePositions(
        int teamsCount,
        ICollection<int>? promotionPositions,
        ICollection<int>? promotionPlayOffPositions,
        ICollection<int>? relegationPositions,
        ICollection<int>? relegationPlayOffPositions,
        ICollection<int>? uefaChampionsLeaguePositions,
        ICollection<int>? uefaEuropaLeaguePositions,
        ICollection<int>? uefaConferenceLeaguePositions)
    {
        var allPositions = new[]
        {
            promotionPositions,
            promotionPlayOffPositions,
            relegationPositions,
            relegationPlayOffPositions,
            uefaChampionsLeaguePositions,
            uefaEuropaLeaguePositions,
            uefaConferenceLeaguePositions
        }
        .Where(c => c != null)
        .SelectMany(c => c!)
        .ToList();

        var invalidPositions = allPositions
            .Where(p => p < 1 || p > teamsCount)
            .Distinct()
            .OrderBy(p => p)
            .ToList();
        if (invalidPositions.Count != 0)
            throw new ArgumentException($"League positions must be between 1 and {teamsCount}. Invalid positions: {string.Join(", ", invalidPositions)}");

        var duplicates = allPositions
            .GroupBy(p => p)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .OrderBy(p => p)
            .ToList();
        if (duplicates.Count != 0)
            throw new ArgumentException($"Duplicate league positions are not allowed. Duplicates: {string.Join(", ", duplicates)}");
    }
}
