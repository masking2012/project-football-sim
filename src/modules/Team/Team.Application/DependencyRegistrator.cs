using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Team.Application.Features;

namespace ProjectFootballSim.Team.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddTeamApplication(
        this IServiceCollection services)
    {
        services.AddScoped<GetTeamsByCountryFeature>();

        return services;
    }
}
