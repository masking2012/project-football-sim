using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Leagues.Application.CreateGameLeague;
using ProjectFootballSim.Leagues.Application.GetLeagueById;
using ProjectFootballSim.Leagues.Application.GetLeagues;
using ProjectFootballSim.Leagues.Application.GetLeaguesByCountries;

namespace ProjectFootballSim.Leagues.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLeaguesApplication(this IServiceCollection services)
    {
        services.AddScoped<GetLeaguesQueryHandler>();
        services.AddScoped<GetLeagueByIdQueryHandler>();

        services.AddScoped<GetGameLeagueStandingsQueryHandler>();
        services.AddScoped<CreateGameLeagueCommandHandler>();

        return services;
    }
}
