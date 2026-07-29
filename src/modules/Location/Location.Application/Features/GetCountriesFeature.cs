using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Location.Infrastructure.Database;

namespace ProjectFootballSim.Location.Application.Features;

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
