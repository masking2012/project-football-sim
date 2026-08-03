using ProjectFootballSim.Common.Data.Helpers;
using System.Text.Json;

namespace ProjectFootballSim.Common.Data.Entities.Leagues;

public static class LeagueDataProvider
{
    public static IReadOnlyList<LeagueData> GetLeaguesByCountryId(int countryId)
    {
        using var stream = ResourceHelper.GetEmbeddedResource($"leagues_{countryId}");
        return JsonSerializer.Deserialize<List<LeagueData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];
    }

    public static IReadOnlyList<LeagueTeamData> GetLeagueTeamsByCountryId(int countryId)
    {
        using var stream = ResourceHelper.GetEmbeddedResource($"league_teams_{countryId}");
        return JsonSerializer.Deserialize<List<LeagueTeamData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];
    }
}
