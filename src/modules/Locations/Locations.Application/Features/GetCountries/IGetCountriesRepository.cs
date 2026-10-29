using ProjectFootballSim.Locations.Contracts.Features.GetCountries;

namespace ProjectFootballSim.Locations.Application.Features.GetCountries;

public interface IGetCountriesRepository
{
    Task<IReadOnlyList<CountryDto>> GetAllAsync(CancellationToken cancellationToken);
}
