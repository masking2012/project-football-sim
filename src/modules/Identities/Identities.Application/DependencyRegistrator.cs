using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Identities.Application.Features.Login;
using ProjectFootballSim.Identities.Application.Features.Register;
using ProjectFootballSim.Identities.Domain.Entities;
using ProjectFootballSim.Identities.Infrastructure.Configuration;

namespace ProjectFootballSim.Identities.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddIdentitiesApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptionsWithValidateOnStart<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddScoped<RegisterCommand>();
        services.AddScoped<LoginCommand>();

        return services;
    }
}
