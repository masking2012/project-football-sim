using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Locations.Infrastructure.Database;

namespace ProjectFootballSim.Locations.Application.Features;

public sealed class GetCountriesFeature(LocationDbContext dbContext)
{
    public async Task<IEnumerable<CountryDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var countries = await dbContext.Countries.ToListAsync(cancellationToken).ConfigureAwait(false);
        return countries.Select(c => new CountryDto
        {
            Id = c.Id,
            Name = c.Name
        });
    }
}
