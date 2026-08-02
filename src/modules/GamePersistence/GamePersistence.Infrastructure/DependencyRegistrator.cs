using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.GamePersistence.Infrastructure.Database;

namespace ProjectFootballSim.GamePersistence.Infrastructure;

public static class DependencyRegistrator
{
    public static IServiceCollection AddGamePersistenceInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringSectionName)
    {
        services.AddDbContext<GamePersistenceDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString(connectionStringSectionName),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        return services;
    }
}
