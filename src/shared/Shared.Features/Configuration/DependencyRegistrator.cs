using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Shared.Features.Random;

namespace ProjectFootballSim.Shared.Features;

public static class DependencyRegistrator
{
    public static IServiceCollection AddCommonFeatures(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IRandomNumberGenerator, CryptoRandom>();
        return services;
    }
}
