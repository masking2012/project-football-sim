using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ProjectFootballSim.Common.Data.Helpers;

internal static class ResourceHelper
{
    public static Stream GetEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fullResourceName = $"ProjectFootballSim.Common.Data.Data.{resourceName}.json";

        var stream = assembly.GetManifestResourceStream(fullResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{fullResourceName}' was not found.");

        return stream;
    }
}
