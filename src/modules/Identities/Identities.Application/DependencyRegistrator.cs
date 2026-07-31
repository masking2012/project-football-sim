using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Identities.Application.Features.Login;
using ProjectFootballSim.Identities.Application.Features.Register;
using ProjectFootballSim.Identities.Domain.Entities;

namespace ProjectFootballSim.Identities.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddIdentitiesApplication(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<RegisterCommand>();
        services.AddScoped<LoginCommand>();

        return services;
    }
}
