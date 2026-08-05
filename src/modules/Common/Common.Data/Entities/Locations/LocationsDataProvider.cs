using ProjectFootballSim.Common.Data.Helpers;
using System.Text.Json;

namespace ProjectFootballSim.Common.Data.Entities.Locations;

public static class LocationsDataProvider
{
    public static IReadOnlyList<CountryData> GetCountries()
    {
        using var stream = ResourceHelper.GetEmbeddedResource("countries");
        return JsonSerializer.Deserialize<List<CountryData>>(stream, ResourceHelper.DefaultJsonOptions) ?? [];
    }
}
