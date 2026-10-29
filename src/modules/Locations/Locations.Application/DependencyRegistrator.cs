using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Locations.Application.Features.GetCountries;
using ProjectFootballSim.Locations.Contracts.Features.GetCountries;

namespace ProjectFootballSim.Locations.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLocationsApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetCountriesQueryHandler, GetCountriesQueryHandler>();
        return services;
    }
}
