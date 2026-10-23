using ProjectFootballSim.Shared.Data.Helpers;
using System.Text.Json;

namespace ProjectFootballSim.Shared.Data.Entities.Locations;

public static class LocationsDataProvider
{
    public static IReadOnlyList<CountryData> GetCountries()
    {
        using var stream = ResourceHelper.GetEmbeddedResource("countries");
        return JsonSerializer.Deserialize<List<CountryData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];
    }
}
