using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Locations.Contracts.Features.GetCountries;

namespace ProjectFootballSim.Locations.Application.Features.GetCountries;

internal sealed class GetCountriesQueryHandler(HybridCache cache, IGetCountriesRepository repository) : IGetCountriesQueryHandler
{
    public ValueTask<IReadOnlyList<CountryDto>> HandleAsync(CancellationToken cancellationToken)
    {
        return cache.GetOrCreateAsync(
            "countries",
            async lambdaCancellationToken => await repository.GetAllAsync(lambdaCancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken
        );
    }
}
