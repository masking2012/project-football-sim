using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Application.Features.GetGameSeasons;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Calendar.Infrastructure.Repositories;

namespace ProjectFootballSim.Calendar.Infrastructure;

public static class DependencyRegistrator
{
    public static IServiceCollection AddCalendarInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringSectionName)
    {
        services.AddDbContext<CalendarDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString(connectionStringSectionName),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        services.AddScoped<ICreateGameCalendarRepository, CreateGameCalendarRepository>();
        services.AddScoped<ICreateGameSeasonRepository, CreateGameSeasonRepository>();
        services.AddScoped<IGetGameSeasonsRepository, GetGameSeasonsRepository>();
        services.AddScoped<IGameCalendarRepository, GameCalendarRepository>();
        services.AddScoped<IGameCalendarWriteRepository, GameCalendarWriteRepository>();

        return services;
    }
}
