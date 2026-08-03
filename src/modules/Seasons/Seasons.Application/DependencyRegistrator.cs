using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Seasons.Application.Features.CreatePlayerSeason;
using ProjectFootballSim.Seasons.Application.Features.GetPlayerSeasons;

namespace ProjectFootballSim.Seasons.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddSeasonsApplication(this IServiceCollection services)
    {
        services.AddScoped<CreatePlayerSeasonCommandHandler>();
        services.AddScoped<GetPlayerSeasonsQueryHandler>();

        return services;
    }
}
