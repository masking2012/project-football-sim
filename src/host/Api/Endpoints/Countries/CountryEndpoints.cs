using ProjectFootballSim.Locations.Application.Features.GetCountries;
using System.Globalization;

namespace ProjectFootballSim.Api.Endpoints.Countries;

internal static class CountriesEndpoints
{
    public static void MapCountriesEndpoints(this WebApplication app)
    {
        app.MapGet("/api/countries", async (GetCountriesQuery getCountriesQuery, CancellationToken cancellationToken) =>
        {
            var countries = await getCountriesQuery.HandleAsync(cancellationToken).ConfigureAwait(false);
            var countryItems = countries.Select(c => new CountryItemResponse(c.Id.ToString(CultureInfo.InvariantCulture), c.Name)).ToList();
            return Results.Ok(countryItems);
        });
    }
}
