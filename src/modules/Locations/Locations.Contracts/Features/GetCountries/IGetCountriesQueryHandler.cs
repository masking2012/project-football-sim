namespace ProjectFootballSim.Locations.Contracts.Features.GetCountries;

public interface IGetCountriesQueryHandler
{
    Task<IReadOnlyList<GetCountryDto>> HandleAsync(CancellationToken cancellationToken);
}
