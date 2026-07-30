using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Locations.Domain.Entities;
using ProjectFootballSim.Locations.Infrastructure.Database;

namespace ProjectFootballSim.Locations.Application.Features.GetCountries;

public sealed class GetCountriesQuery(LocationsDbContext dbContext)
{
    public async Task<IEnumerable<CountryDto>> HandleAsync(CancellationToken cancellationToken)
    {
        List<Country> countries = await dbContext.Countries.ToListAsync(cancellationToken).ConfigureAwait(false);
        return countries.Select(c => new CountryDto(c.Id, c.Name));
    }
}
