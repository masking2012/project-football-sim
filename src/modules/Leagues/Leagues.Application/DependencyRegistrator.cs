using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Leagues.Application.Common.Services;
using ProjectFootballSim.Leagues.Application.Features.GetLeagueById;
using ProjectFootballSim.Leagues.Application.Features.GetLeagues;
using ProjectFootballSim.Leagues.Application.GameFeatures.CreateGameLeague;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueFixtures;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueStandings;
using ProjectFootballSim.Leagues.Application.GameFeatures.SimulateLeagueGameDay;

namespace ProjectFootballSim.Leagues.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLeaguesApplication(this IServiceCollection services)
    {
        services.AddScoped<ILeaguesCatalog, LeaguesCatalog>();
        services.AddScoped<ILeaguesRoundsCatalog, LeaguesRoundsCatalog>();

        services.AddScoped<GetLeaguesQueryHandler>();
        services.AddScoped<GetLeagueByIdQueryHandler>();

        services.AddScoped<GetGameLeagueStandingsQueryHandler>();
        services.AddScoped<GetGameLeagueFixturesQueryHandler>();
        services.AddScoped<GetGameLeagueMatchesByDateQueryHandler>();

        services.AddScoped<LeagueFixtureGenerator>();
        services.AddScoped<CreateGameLeagueCommandHandler>();
        services.AddScoped<SimulateLeagueGameDayCommandHandler>();

        return services;
    }
}
