using ProjectFootballSim.Common.Data.Helpers;
using System.Text.Json;

namespace ProjectFootballSim.Common.Data.Entities.Countries;

public static class CountryDataProvider
{
    //private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static IReadOnlyList<CountryData> GetAll()
    {
        using var stream = ResourceHelper.GetEmbeddedResource("countries");
        return JsonSerializer.Deserialize<List<CountryData>>(stream) ?? [];
    }
}
