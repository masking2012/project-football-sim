using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Match.Application.Common.Services;
using ProjectFootballSim.Match.Application.Features.ExtraTime;
using ProjectFootballSim.Match.Application.Features.RegularTime;

namespace ProjectFootballSim.Match.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddMatchApplication(this IServiceCollection services)
    {
        services.AddScoped<IPossessionCalculator, PossessionCalculator>();
        services.AddScoped<IChancesCalculator, ChancesCalculator>();
        services.AddScoped<IGoalsCalculator, GoalsCalculator>();

        services.AddScoped<ExtraTimeSimulator>();
        services.AddScoped<RegularTimeSimulator>();
        return services;
    }
}
