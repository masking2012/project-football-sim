using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Common.Data.Entities.Countries;
using ProjectFootballSim.Locations.Domain.Entities;
using ProjectFootballSim.Locations.Infrastructure.Database;

namespace ProjectFootballSim.Locations.Infrastructure;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLocationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringSectionName)
    {
        services.AddDbContext<LocationDbContext>(options =>
            options.UseSqlServer(
            configuration.GetConnectionString(connectionStringSectionName),
            sqlOptions => sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null))
            .UseSeeding(SeedWithPredefinedValues)
        );

        return services;
    }

    private static void SeedWithPredefinedValues(DbContext context, bool storeManagementOpetationWasPerformed)
    {
        foreach (var countryData in CountryDataProvider.GetAll())
        {
            var country = context.Set<Country>().SingleOrDefault(c => c.Id == countryData.Id);
            if (country is null)
                context.Set<Country>().Add(new Country(countryData.Id, countryData.Name));
        }
        context.SaveChanges();
    }
}
