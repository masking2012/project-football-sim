using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Locations.Domain.Entities;
using ProjectFootballSim.Locations.Infrastructure.Database;

namespace ProjectFootballSim.Locations.Application.Features.GetCountries;

public sealed class GetCountriesQuery(HybridCache cache, LocationsDbContext dbContext)
{
    public ValueTask<IEnumerable<CountryDto>> HandleAsync(CancellationToken cancellationToken)
    {
        return cache.GetOrCreateAsync(
            "countries",
            async cancel => await GetDataFromTheSourceAsync(cancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken
        );
    }

    private async Task<IEnumerable<CountryDto>> GetDataFromTheSourceAsync(CancellationToken cancellationToken)
    {
        List<Country> countries = await dbContext.Countries.ToListAsync(cancellationToken).ConfigureAwait(false);
        return countries.Select(c => new CountryDto(c.Id, c.Name));
    }
}
