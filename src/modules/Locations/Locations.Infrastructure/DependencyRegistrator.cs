using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Locations.Application.Features.GetCountries;
using ProjectFootballSim.Locations.Domain.Entities;
using ProjectFootballSim.Locations.Infrastructure.Database;
using ProjectFootballSim.Locations.Infrastructure.Repositories;
using ProjectFootballSim.Shared.Data.Entities.Locations;

namespace ProjectFootballSim.Locations.Infrastructure;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLocationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringSectionName)
    {
        services.AddDbContext<LocationsDbContext>(options =>
            options.UseSqlServer(
            configuration.GetConnectionString(connectionStringSectionName),
            sqlOptions => sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null))
            .UseSeeding(SeedWithPredefinedValues)
        );

        services.AddScoped<IGetCountriesRepository, GetCountriesRepository>();
        return services;
    }

    private static void SeedWithPredefinedValues(DbContext context, bool storeManagementOpetationWasPerformed)
    {
        foreach (var countryData in LocationsDataProvider.GetCountries())
        {
            var country = context.Set<Country>().SingleOrDefault(c => c.Id == countryData.Id);
            if (country is null)
                context.Set<Country>().Add(new Country(countryData.Id, countryData.Name, countryData.Code));
        }
        context.SaveChanges();
    }
}
