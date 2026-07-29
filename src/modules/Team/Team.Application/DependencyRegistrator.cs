using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Team.Application.Features.GetTeamById;
using ProjectFootballSim.Team.Application.Features.GetTeamsByCountry;

namespace ProjectFootballSim.Team.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddTeamApplication(
        this IServiceCollection services)
    {
        services.AddScoped<GetTeamsByCountryFeature>();
        services.AddScoped<GetTeamByIdFeature>();

        return services;
    }
}
