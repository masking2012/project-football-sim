using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Teams.Application.Common.Interfaces;
using ProjectFootballSim.Teams.Application.Common.Services;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByIds;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByCountry;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;

namespace ProjectFootballSim.Teams.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddTeamsApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ICacheTeamsRetriever, CacheTeamsRetriever>();
        services.AddScoped<IGetTeamsByCountryQueryHandler, GetTeamsByCountryQueryHandler>();
        services.AddScoped<IGetTeamsByIdsQueryHandler, GetTeamsByIdsQueryHandler>();

        return services;
    }
}
