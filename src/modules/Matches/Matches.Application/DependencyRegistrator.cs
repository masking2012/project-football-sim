using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Application.Features.ExtraTime;
using ProjectFootballSim.Matches.Application.Features.Penalty;
using ProjectFootballSim.Matches.Application.Features.RegularTime;
using ProjectFootballSim.Matches.Domain.Services;

namespace ProjectFootballSim.Matches.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddMatchesApplication(this IServiceCollection services)
    {
        services.AddScoped<IPossessionCalculator, PossessionCalculator>();
        services.AddScoped<IChancesCalculator, ChancesCalculator>();
        services.AddScoped<IGoalsCalculator, GoalsCalculator>();

        services.AddScoped<SimulateRegularTimeCommand>();
        services.AddScoped<SimulateExtraTimeCommand>();
        services.AddScoped<SimulatePenaltyShootoutCommand>();
        return services;
    }
}
