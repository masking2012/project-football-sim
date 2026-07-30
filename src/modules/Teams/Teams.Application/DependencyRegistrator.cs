using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Teams.Application.Features.GetTeamById;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;

namespace ProjectFootballSim.Teams.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddTeamsApplication(
        this IServiceCollection services)
    {
        services.AddScoped<GetTeamsByCountryQuery>();
        services.AddScoped<GetTeamByIdQuery>();

        return services;
    }
}
