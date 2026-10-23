using ProjectFootballSim.Shared.Data.Helpers;
using System.Text.Json;

namespace ProjectFootballSim.Shared.Data.Entities.Seasons;

public static class SeasonsDataProvider
{
    public static SeasonDefinitionData GetSeasonDefinition()
    {
        using var stream = ResourceHelper.GetEmbeddedResource($"season_definition");
        return JsonSerializer.Deserialize<SeasonDefinitionData>(stream, ResourceHelper.DefaultJsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize season definition data.");
    }
}
