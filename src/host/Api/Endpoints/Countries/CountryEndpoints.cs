using ProjectFootballSim.Api.Endpoints.Countries;
using ProjectFootballSim.Locations.Application.Features;
using System.Globalization;

namespace ProjectFootballSim.Api.Endpoints.Countries;

internal static class CountryEndpoints
{
    public static void MapCountryEndpoints(this WebApplication app)
    {
        app.MapGet("/api/countries", async (GetCountriesFeature getCountriesFeature, CancellationToken cancellationToken) =>
        {
            var countries = await getCountriesFeature.HandleAsync(cancellationToken).ConfigureAwait(false);
            var countryItems = countries.Select(c => new CountryItemResponse(c.Id.ToString(CultureInfo.InvariantCulture), c.Name)).ToList();
            return Results.Ok(countryItems);
        });
    }
}
