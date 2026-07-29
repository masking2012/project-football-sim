using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Location.Application.Features;

namespace ProjectFootballSim.Location.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLocationApplication(this IServiceCollection services)
    {
        services.AddScoped<GetCountriesFeature>();
        return services;
    }
}
