using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Leagues.Application.Catalog.Features.GetActiveLeagues;
using ProjectFootballSim.Leagues.Application.Catalog.Features.GetLeagueByVersionId;
using ProjectFootballSim.Leagues.Application.Catalog.Services;
using ProjectFootballSim.Leagues.Application.GameState.Features.CreateGameLeague;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueFixtures;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagues;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueStandings;
using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Application.GameState.Features.SimulateLeagueGameDay;

namespace ProjectFootballSim.Leagues.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLeaguesApplication(this IServiceCollection services)
    {
        services.AddScoped<ICachedLeaguesRetriever, CachedLeaguesRetriever>();
        services.AddScoped<ILeaguesRoundsCatalog, LeaguesRoundsCatalog>();
        services.AddScoped<IGameLeagueCatalog, GameLeagueCatalog>();

        services.AddScoped<GetActiveLeaguesQueryHandler>();
        services.AddScoped<GetLeagueByVersionIdQueryHandler>();

        services.AddScoped<GetGameLeagueStandingsQueryHandler>();
        services.AddScoped<GetGameLeagueFixturesQueryHandler>();
        services.AddScoped<GetGameLeagueMatchesByDateQueryHandler>();
        services.AddScoped<GetGameLeaguesQueryHandler>();

        services.AddScoped<LeagueFixtureGenerator>();
        services.AddScoped<CreateGameLeagueCommandHandler>();
        services.AddScoped<SimulateLeagueGameDayCommandHandler>();

        return services;
    }
}
