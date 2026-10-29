using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Locations.Contracts.Features.GetCountries;

namespace ProjectFootballSim.Locations.Application.Features.GetCountries;

internal sealed class GetCountriesQueryHandler(HybridCache cache, IGetCountriesRepository repository) : IGetCountriesQueryHandler
{
    public async Task<IReadOnlyList<GetCountryDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var cachedCountries = await GetCachedCountriesAsync(cancellationToken).ConfigureAwait(false);
        return cachedCountries.Select(country => new GetCountryDto(country.Id, country.Name, country.Code)).ToList();
    }

    private ValueTask<IReadOnlyList<CountryDto>> GetCachedCountriesAsync(CancellationToken cancellationToken)
    {
        return cache.GetOrCreateAsync(
            "countries",
            async lambdaCancellationToken => await repository.GetAllAsync(lambdaCancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken
        );
    }
}
