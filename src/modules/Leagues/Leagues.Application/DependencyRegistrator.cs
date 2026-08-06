using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Leagues.Application.Features.GetLeagueById;
using ProjectFootballSim.Leagues.Application.Features.GetLeagues;
using ProjectFootballSim.Leagues.Application.GameFeatures.CreateGameLeague;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueStandings;

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
