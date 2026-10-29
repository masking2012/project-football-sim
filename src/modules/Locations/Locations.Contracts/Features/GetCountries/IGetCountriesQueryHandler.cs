namespace ProjectFootballSim.Locations.Contracts.Features.GetCountries;

public interface IGetCountriesQueryHandler
{
    ValueTask<IReadOnlyList<CountryDto>> HandleAsync(CancellationToken cancellationToken);
}
