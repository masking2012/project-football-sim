using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Leagues.Application.GetLeaguesByCountries;

namespace ProjectFootballSim.Leagues.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLeaguesApplication(this IServiceCollection services)
    {
        services.AddScoped<GetLeaguesByCountriesQueryHandler>();

        return services;
    }
}
