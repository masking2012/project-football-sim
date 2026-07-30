using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Locations.Application.Features;

namespace ProjectFootballSim.Locations.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLocationsApplication(this IServiceCollection services)
    {
        services.AddScoped<GetCountriesFeature>();
        return services;
    }
}
