using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Seasons.Application.Features.GetCurrentSeason;
using ProjectFootballSim.Seasons.Application.Features.GetSeasonDefinition;

namespace ProjectFootballSim.Seasons.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddSeasonsApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateNextPlayerSeasonCommandHandler>();
        services.AddScoped<GetCurrentPlayerSeasonQueryHandler>();

        return services;
    }
}
