using ProjectFootballSim.Common.Data.Helpers;
using System.Text.Json;

namespace ProjectFootballSim.Common.Data.Entities.Leagues;

public static class LeaguesDataProvider
{
    public static IReadOnlyList<LeagueData> GetLeaguesByCountryId(int countryId)
    {
        using var stream = ResourceHelper.GetEmbeddedResource($"leagues_{countryId}");
        return JsonSerializer.Deserialize<List<LeagueData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];
    }

    public static IReadOnlyList<LeagueTeamData> GetLeagueTeamsByCountryId(int countryId)
    {
        using var stream = ResourceHelper.GetEmbeddedResource($"leagues_teams_{countryId}");
        var result = JsonSerializer.Deserialize<List<LeagueTeamData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];

        var uniquePairs = new HashSet<Tuple<int, int>>();
        foreach (var item in result)
        {
            if (!uniquePairs.Add(Tuple.Create(item.LeagueId, item.TeamId)))
                throw new InvalidDataException($"GetLeagueTeamsByCountryId. Duplicate entry found for LeagueId: {item.LeagueId}, TeamId: {item.TeamId}");
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
        var uniqueLeagueDates = new HashSet<Tuple<int, int, bool>>();
        var uniqueLeagueRounds = new HashSet<Tuple<int, int>>();
        foreach (var item in items)
        {
            if (!uniqueLeagueDates.Add(Tuple.Create(item.LeagueId, item.Week, item.IsMidweek)))
                throw new InvalidDataException($"GetLeagueRoundsByCountryId. Duplicate entry found for LeagueId: {item.LeagueId}, Week: {item.Week}, IsMidweek: {item.IsMidweek}");
            if (!uniqueLeagueRounds.Add(Tuple.Create(item.LeagueId, item.Round)))
                throw new InvalidDataException($"GetLeagueRoundsByCountryId. Duplicate entry found for LeagueId: {item.LeagueId}, Round: {item.Round}");
        }

        foreach (var leagueRounds in items.GroupBy(item => item.LeagueId))
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
