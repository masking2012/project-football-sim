namespace ProjectFootballSim.Leagues.Domain.Entities;

public sealed class League
{
    public int Id { get; }
    public string Name { get; private set; }
    public int Order { get; private set; }
    public int CountryId { get; private set; }
    public int TeamsCount { get; private set; }
    public ICollection<int>? PromotionPositions { get; private set; }
    public ICollection<int>? PromotionPlayOffPositions { get; private set; }
    public ICollection<int>? RelegationPositions { get; private set; }
    public ICollection<int>? RelegationPlayOffPositions { get; private set; }
    public ICollection<int>? UefaChampionsLeaguePositions { get; private set; }
    public ICollection<int>? UefaEuropaLeaguePositions { get; private set; }
    public ICollection<int>? UefaConferenceLeaguePositions { get; private set; }

    public League(
        int id,
        string name,
        int order,
        int countryId,
        int teamsCount,
        ICollection<int>? promotionPositions,
        ICollection<int>? promotionPlayOffPositions,
        ICollection<int>? relegationPositions,
        ICollection<int>? relegationPlayOffPositions,
        ICollection<int>? uefaChampionsLeaguePositions,
        ICollection<int>? uefaEuropaLeaguePositions,
        ICollection<int>? uefaConferenceLeaguePositions)
    {
        if (id <= 0)
            throw new ArgumentException("League ID must be positive value");
        ValidateData(
            name,
            order,
            countryId,
            teamsCount,
            promotionPositions,
            promotionPlayOffPositions,
            relegationPositions,
            relegationPlayOffPositions,
            uefaChampionsLeaguePositions,
            uefaEuropaLeaguePositions,
            uefaConferenceLeaguePositions);

        Id = id;
        Name = name;
        Order = order;
        CountryId = countryId;
        TeamsCount = teamsCount;
        PromotionPositions = promotionPositions;
        PromotionPlayOffPositions = promotionPlayOffPositions;
        RelegationPositions = relegationPositions;
        RelegationPlayOffPositions = relegationPlayOffPositions;
        UefaChampionsLeaguePositions = uefaChampionsLeaguePositions;
        UefaEuropaLeaguePositions = uefaEuropaLeaguePositions;
        UefaConferenceLeaguePositions = uefaConferenceLeaguePositions;
    }

    public void Update(
        string name,
        int order,
        int countryId,
        int teamsCount,
        ICollection<int>? promotionPositions,
        ICollection<int>? promotionPlayOffPositions,
        ICollection<int>? relegationPositions,
        ICollection<int>? relegationPlayOffPositions,
        ICollection<int>? uefaChampionsLeaguePositions,
        ICollection<int>? uefaEuropaLeaguePositions,
        ICollection<int>? uefaConferenceLeaguePositions)
    {
        ValidateData(
            name,
            order,
            countryId,
            teamsCount,
            promotionPositions,
            promotionPlayOffPositions,
            relegationPositions,
            relegationPlayOffPositions,
            uefaChampionsLeaguePositions,
            uefaEuropaLeaguePositions,
            uefaConferenceLeaguePositions);

        Name = name;
        Order = order;
        CountryId = countryId;
        TeamsCount = teamsCount;
        PromotionPositions = promotionPositions;
        PromotionPlayOffPositions = promotionPlayOffPositions;
        RelegationPositions = relegationPositions;
        RelegationPlayOffPositions = relegationPlayOffPositions;
        UefaChampionsLeaguePositions = uefaChampionsLeaguePositions;
        UefaEuropaLeaguePositions = uefaEuropaLeaguePositions;
        UefaConferenceLeaguePositions = uefaConferenceLeaguePositions;
    }

    private static void ValidateData(
        string name,
        int order,
        int countryId,
        int teamsCount,
        ICollection<int>? promotionPositions,
        ICollection<int>? promotionPlayOffPositions,
        ICollection<int>? relegationPositions,
        ICollection<int>? relegationPlayOffPositions,
        ICollection<int>? uefaChampionsLeaguePositions,
        ICollection<int>? uefaEuropaLeaguePositions,
        ICollection<int>? uefaConferenceLeaguePositions)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("League name cannot be null or empty");
        if (order <= 0)
            throw new ArgumentException("League order must be positive value");
        if (countryId <= 0)
            throw new ArgumentException("Country ID must be positive value");
        if (teamsCount < 2)
            throw new ArgumentException("Teams count in league shouldn't be less than 2");

        ValidatePositions(
            teamsCount,
            promotionPositions,
            promotionPlayOffPositions,
            relegationPositions,
            relegationPlayOffPositions,
            uefaChampionsLeaguePositions,
            uefaEuropaLeaguePositions,
            uefaConferenceLeaguePositions);
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

        // Validate range
        var invalidPositions = allPositions
            .Where(p => p < 1 || p > teamsCount)
            .Distinct()
            .OrderBy(p => p)
            .ToList();

        if (invalidPositions.Count != 0)
            throw new ArgumentException(
                $"League positions must be between 1 and {teamsCount}. Invalid positions: {string.Join(", ", invalidPositions)}");

        // Validate duplicates
        var duplicates = allPositions
            .GroupBy(p => p)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .OrderBy(p => p)
            .ToList();

        if (duplicates.Count != 0)
            throw new ArgumentException(
                $"Duplicate league positions are not allowed. Duplicates: {string.Join(", ", duplicates)}");
    }
}
