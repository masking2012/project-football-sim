using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Application.Features.UpdateGameCalendarDate;

namespace ProjectFootballSim.Calendar.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddCalendarApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateGameCalendarCommandHandler>();
        services.AddScoped<UpdateGameCalendarDateCommandHandler>();

        return services;
    }
}
