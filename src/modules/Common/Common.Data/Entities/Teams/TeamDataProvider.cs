using ProjectFootballSim.Common.Data.Helpers;
using System.Text.Json;

namespace ProjectFootballSim.Common.Data.Entities.Teams;

public static class TeamDataProvider
{
    public static IReadOnlyList<TeamData> GetAll()
    {
        using var stream = ResourceHelper.GetEmbeddedResource("teams_1");
        return JsonSerializer.Deserialize<List<TeamData>>(stream) ?? [];
    }
}
