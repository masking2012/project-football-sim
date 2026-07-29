using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Common.Data.Entities.Countries;
using ProjectFootballSim.Location.Domain.Entities;
using ProjectFootballSim.Location.Infrastructure.Database;

namespace ProjectFootballSim.Location.Infrastructure;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLocationInfrastructure(this IServiceCollection services, IConfiguration configuration, string connectionStringSectionName)
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
            var country = context.Set<CountryEntity>().SingleOrDefault(c => c.Id == countryData.Id);
            if (country is null)
                context.Set<CountryEntity>().Add(new CountryEntity(countryData.Id, countryData.Name));
        }
        context.SaveChanges();
    }
}
