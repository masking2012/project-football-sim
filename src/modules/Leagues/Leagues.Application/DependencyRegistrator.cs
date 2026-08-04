using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Leagues.Application.GetLeaguesByCountries;

namespace ProjectFootballSim.Leagues.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddSeasonsApplication(this IServiceCollection services)
    {
        services.AddScoped<GetLeaguesByCountriesQueryHandler>();

        return services;
    }
}
