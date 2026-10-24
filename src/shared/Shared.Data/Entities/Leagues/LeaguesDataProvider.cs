using ProjectFootballSim.Shared.Data.Helpers;
using System.Text.Json;

namespace ProjectFootballSim.Shared.Data.Entities.Leagues;

public static class LeaguesDataProvider
{
    public static IReadOnlyList<LeagueData> GetLeaguesByCountryId(int countryId)
    {
        using var stream = ResourceHelper.GetEmbeddedResource($"leagues_{countryId}");
        return JsonSerializer.Deserialize<List<LeagueData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];
    }

    public static IReadOnlyList<LeagueActivationData> GetLeagueActivationsByCountryId(int countryId)
    {
        using var stream = ResourceHelper.GetEmbeddedResource($"league_activations_{countryId}");
        return JsonSerializer.Deserialize<List<LeagueActivationData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];
    }

    public static IReadOnlyList<LeagueTeamData> GetLeagueTeamsByCountryId(int countryId)
    {
        using var stream = ResourceHelper.GetEmbeddedResource($"leagues_teams_{countryId}");
        var result = JsonSerializer.Deserialize<List<LeagueTeamData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];

        var uniquePairs = new HashSet<Tuple<int, int, int>>();
        foreach (var item in result)
        {
            if (!uniquePairs.Add(Tuple.Create(item.LeagueId, item.Version, item.TeamId)))
                throw new InvalidDataException($"GetLeagueTeamsByCountryId. Duplicate entry found for LeagueId: {item.LeagueId}, Version: {item.Version}, TeamId: {item.TeamId}");
        }

        return result;
    }

    public static IReadOnlyList<LeagueRoundData> GetLeagueRoundsByCountryId(int countryId)
    {
        using var stream = ResourceHelper.GetEmbeddedResource($"leagues_rounds_{countryId}");
        var result = JsonSerializer.Deserialize<List<LeagueRoundData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];
        ValidateLeagueRoundData(result);
        return result;
    }

    private static void ValidateLeagueRoundData(List<LeagueRoundData> items)
    {
        var uniqueLeagueDates = new HashSet<Tuple<int, int, int, bool>>();
        var uniqueLeagueRounds = new HashSet<Tuple<int, int, int>>();
        foreach (var item in items)
        {
            if (!uniqueLeagueDates.Add(Tuple.Create(item.LeagueId, item.Version, item.Week, item.IsMidweek)))
                throw new InvalidDataException($"GetLeagueRoundsByCountryId. Duplicate entry found for LeagueId: {item.LeagueId}, Version: {item.Version}, Week: {item.Week}, IsMidweek: {item.IsMidweek}");
            if (!uniqueLeagueRounds.Add(Tuple.Create(item.LeagueId, item.Version, item.Round)))
                throw new InvalidDataException($"GetLeagueRoundsByCountryId. Duplicate entry found for LeagueId: {item.LeagueId}, Version: {item.Version}, Round: {item.Round}");
        }

        foreach (var leagueRounds in items.GroupBy(item => new { item.LeagueId, item.Version }))
        {
            var orderedRounds = leagueRounds.OrderBy(item => item.Round).ToList();
            for (var i = 1; i < orderedRounds.Count; i++)
            {
                var previousRound = orderedRounds[i - 1];
                var currentRound = orderedRounds[i];
                if (currentRound.Week < previousRound.Week)
                    throw new InvalidDataException($"GetLeagueRoundsByCountryId. Round {currentRound.Round} for LeagueId: {currentRound.LeagueId} has Week: {currentRound.Week}, which is earlier than the previous Round {previousRound.Round} in Week: {previousRound.Week}");
            }
        }
    }
}
