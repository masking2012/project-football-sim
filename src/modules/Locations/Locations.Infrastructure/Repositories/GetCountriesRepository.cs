using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Locations.Application.Features.GetCountries;
using ProjectFootballSim.Locations.Infrastructure.Database;

namespace ProjectFootballSim.Locations.Infrastructure.Repositories;

internal sealed class GetCountriesRepository(LocationsDbContext dbContext) : IGetCountriesRepository
{
    public async Task<IReadOnlyList<CountryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var countries = await dbContext.Countries
            .AsNoTracking()
            .Select(c => new CountryDto(c.Id, c.Name, c.Code))
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        return countries;
    }
}
