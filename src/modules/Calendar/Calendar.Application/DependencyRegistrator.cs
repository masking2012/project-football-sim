using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Calendar.Application.EventListeners;
using ProjectFootballSim.Calendar.Application.Features.AdvanceCalendarDay;
using ProjectFootballSim.Calendar.Application.Features.CompleteGameSeason;
using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;
using ProjectFootballSim.Calendar.Application.Features.GetGameSeasons;
using ProjectFootballSim.Calendar.Application.Features.SimulateCalendarDay;
using ProjectFootballSim.Calendar.Contracts.Features.AdvanceCalendarDay;
using ProjectFootballSim.Calendar.Contracts.Features.CompleteGameSeason;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Contracts.Features.GetCalendarDay;
using ProjectFootballSim.Calendar.Contracts.Features.GetGameSeasons;
using ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;
using ProjectFootballSim.GamePersistence.Contracts.Features.CreateGame;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddCalendarApplication(this IServiceCollection services)
    {
        services.AddScoped<IAdvanceCalendarDayCommandHandler, AdvanceCalendarDayCommandHandler>();
        services.AddScoped<ISimulateCalendarDayCommandHandler, SimulateCalendarDayCommandHandler>();

        services.AddScoped<ICreateGameCalendarCommandHandler, CreateGameCalendarCommandHandler>();
        services.AddScoped<IGetCalendarDayQueryHandler, GetCalendarDayQueryHandler>();

        services.AddScoped<ICreateGameSeasonCommandHandler, CreateGameSeasonCommandHandler>();
        services.AddScoped<ICompleteGameSeasonCommandHandler, CompleteGameSeasonCommandHandler>();
        services.AddScoped<IGetGameSeasonsQueryHandler, GetGameSeasonsQueryHandler>();

        services.AddScoped<IEventListener<GameCreatedEvent>, GameCreatedEventListener>();

        return services;
    }
}
