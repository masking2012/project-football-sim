using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;
using ProjectFootballSim.Leagues.Application.Catalog.Features.GetActiveLeagues;
using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Application.Catalog.Services;
using ProjectFootballSim.Leagues.Application.GameState.EventListeners;
using ProjectFootballSim.Leagues.Application.GameState.Features.CreateGameLeague;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueFixtures;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByIds;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueRoundsToSchedule;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagues;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueStandings;
using ProjectFootballSim.Leagues.Application.GameState.Features.SimulateLeagueGameDay;
using ProjectFootballSim.Leagues.Application.GameState.Features.UpdateGameLeagueRoundStatus;
using ProjectFootballSim.Leagues.Application.GameState.Interfaces;
using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Contracts.CatalogFeatures.GetActiveLeagues;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.CreateGameLeague;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueFixtures;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByDate;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByIds;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueRoundsToSchedule;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueStandings;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.SimulateLeagueGameDay;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.UpdateGameLeagueRoundStatus;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Leagues.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLeaguesApplication(this IServiceCollection services)
    {
        services.AddScoped<ICachedLeaguesRetriever, CachedLeaguesRetriever>();
        services.AddScoped<IGameLeaguesService, GameLeagueService>();

        services.AddScoped<IGetActiveLeaguesQueryHandler, GetActiveLeaguesQueryHandler>();

        services.AddScoped<IGetGameLeagueStandingsQueryHandler, GetGameLeagueStandingsQueryHandler>();
        services.AddScoped<IGetGameLeagueFixturesQueryHandler, GetGameLeagueFixturesQueryHandler>();
        services.AddScoped<IGetGameLeagueMatchesByDateQueryHandler, GetGameLeagueMatchesByDateQueryHandler>();
        services.AddScoped<IGetGameLeagueMatchesByIdsQueryHandler, GetGameLeagueMatchesByIdsQueryHandler>();
        services.AddScoped<IGetGameLeagueRoundsToScheduleQueryHandler, GetGameLeagueRoundsToScheduleQueryHandler>();
        services.AddScoped<GetGameLeaguesQueryHandler>();

        services.AddScoped<LeagueFixtureGenerator>();
        services.AddScoped<ICreateGameLeagueCommandHandler, CreateGameLeagueCommandHandler>();
        services.AddScoped<ISimulateLeagueGameDayCommandHandler, SimulateLeagueGameDayCommandHandler>();
        services.AddScoped<IUpdateGameLeagueRoundStatusCommandHandler, UpdateGameLeagueRoundStatusCommandHandler>();

        services.AddScoped<IEventListener<GameSeasonStartedEvent>, GameSeasonStartedEventListener>();
        services.AddScoped<IEventListener<SimulateCalendarDayEvent>, SimulateCalendarDayEventListener>();

        return services;
    }
}
