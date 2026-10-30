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
using ProjectFootballSim.Leagues.Application.GameState.Interfaces;
using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Shared.Features.Events;
using ProjectFootballSim.Leagues.Contracts.CatalogFeatures.GetActiveLeagues;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueStandings;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueFixtures;
using ProjectFootballSim.Leagues.Application.GameState.EventListeners;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByDate;

namespace ProjectFootballSim.Leagues.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLeaguesApplication(this IServiceCollection services)
    {
        services.AddScoped<ICachedLeaguesRetriever, CachedLeaguesRetriever>();
        services.AddScoped<IGameLeaguesService, GameLeagueService>();

        services.AddScoped<IGetActiveLeaguesQueryHandler, GetActiveLeaguesQueryHandler>();
        services.AddScoped<GetLeagueByVersionIdQueryHandler>();

        services.AddScoped<IGetGameLeagueStandingsQueryHandler, GetGameLeagueStandingsQueryHandler>();
        services.AddScoped<IGetGameLeagueFixturesQueryHandler, GetGameLeagueFixturesQueryHandler>();
        services.AddScoped<IGetGameLeagueMatchesByDateQueryHandler, GetGameLeagueMatchesByDateQueryHandler>();
        services.AddScoped<GetGameLeaguesQueryHandler>();

        services.AddScoped<LeagueFixtureGenerator>();
        services.AddScoped<CreateGameLeagueCommandHandler>();
        services.AddScoped<SimulateLeagueGameDayCommandHandler>();
        services.AddScoped<IEventListener<GameSeasonStartedEvent>, GameSeasonStartedEventListener>();

        return services;
    }
}
