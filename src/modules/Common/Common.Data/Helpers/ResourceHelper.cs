using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectFootballSim.Common.Data.Helpers;

internal static class ResourceHelper
{
    public static JsonSerializerOptions DefaultJsonOptions => CreateDefaultJsonOptions();

    private static JsonSerializerOptions CreateDefaultJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }

    public static Stream GetEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fullResourceName = $"ProjectFootballSim.Common.Data.Files.{resourceName}.json";

        var stream = assembly.GetManifestResourceStream(fullResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{fullResourceName}' was not found.");

        return stream;
    }
}
