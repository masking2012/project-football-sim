using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Application.Features.GetDayWithEvents;
using ProjectFootballSim.Calendar.Application.Features.ProceedCalendar;
using ProjectFootballSim.Calendar.Application.Features.SimulateGameDay;
using ProjectFootballSim.Calendar.Application.Features.UpdateGameCalendarDate;

namespace ProjectFootballSim.Calendar.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddCalendarApplication(this IServiceCollection services)
    {
        services.AddScoped<IGameCalendarRetriever, GameCalendarRetriever>();

        services.AddScoped<ProceedCalendarCommandHandler>();
        services.AddScoped<SimulateGameDayCommandHandler>();

        services.AddScoped<CreateGameCalendarCommandHandler>();
        services.AddScoped<UpdateGameCalendarDateCommandHandler>();
        services.AddScoped<GetDayWithEventsQueryHandler>();

        return services;
    }
}
