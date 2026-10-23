using Microsoft.Extensions.DependencyInjection;

namespace ProjectFootballSim.Common.Features;

public static class DependencyRegistrator
{
    public static IServiceCollection AddCommonFeatures(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IRandomNumberGenerator, CryptoRandom>();
        return services;
    }
}
