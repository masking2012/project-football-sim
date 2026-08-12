using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Application.Features.AdvanceCalendarDay;
using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;
using ProjectFootballSim.Calendar.Application.Features.GetGameSeasons;
using ProjectFootballSim.Calendar.Application.Features.SimulateCalendarDay;

namespace ProjectFootballSim.Calendar.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddCalendarApplication(this IServiceCollection services)
    {
        services.AddScoped<IGameCalendarRetriever, GameCalendarRetriever>();
        services.AddScoped<ISeasonsRetriever, SeasonsRetriever>();

        services.AddScoped<AdvanceCalendarDayCommandHandler>();
        services.AddScoped<SimulateCalendarDayCommandHandler>();

        services.AddScoped<CreateGameCalendarCommandHandler>();
        services.AddScoped<GetCalendarDayQueryHandler>();

        services.AddScoped<CreateGameSeasonCommandHandler>();
        services.AddScoped<GetGameSeasonsQueryHandler>();

        return services;
    }
}
