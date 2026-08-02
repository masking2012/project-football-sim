using ProjectFootballSim.Common.Data.Helpers;
using System.Text.Json;

namespace ProjectFootballSim.Common.Data.Entities.Teams;

public static class TeamDataProvider
{
    public static IReadOnlyList<TeamData> GetAll(int countryId)
    {
        using var stream = ResourceHelper.GetEmbeddedResource($"teams_{countryId}");
        return JsonSerializer.Deserialize<List<TeamData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];
    }
}
