using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.GamePersistence.Application.Features.LoadGames;
using ProjectFootballSim.GamePersistence.Application.Features.SaveGame;

namespace ProjectFootballSim.GamePersistence.Application;

public static class DependencyRegistrator
{
    public static IServiceCollection AddGamePersistenceApplication(this IServiceCollection services)
    {
        services.AddScoped<SaveGameCommandHandler>();
        services.AddScoped<LoadGamesQueryHandler>();

        return services;
    }
}
