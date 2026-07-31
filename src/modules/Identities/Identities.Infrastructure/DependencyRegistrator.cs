using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Identities.Infrastructure.Configuration;
using ProjectFootballSim.Identities.Infrastructure.Database;
using ProjectFootballSim.Identities.Infrastructure.Services;

namespace ProjectFootballSim.Identities.Infrastructure;

public static class DependencyRegistrator
{
    public static IServiceCollection AddIdentitiesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringSectionName)
    {
        services.AddDbContext<IdentitiesDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString(connectionStringSectionName),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        services
            .AddOptionsWithValidateOnStart<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<JwtTokenGenerator>();

        return services;
    }
}
