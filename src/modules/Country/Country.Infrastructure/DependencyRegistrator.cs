using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Common.Data.Entities.Countries;
using ProjectFootballSim.Country.Domain.Entities;
using ProjectFootballSim.Country.Infrastructure.Database;

namespace ProjectFootballSim.Country.Infrastructure;

public static class DependencyRegistrator
{
    public static IServiceCollection AddCountryInfrastructure(this IServiceCollection services, IConfiguration configuration, string connectionStringSectionName)
    {
        services.AddDbContext<CountryDbContext>(options =>
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
            var country = context.Set<CountryEntity>().SingleOrDefault(c => c.Name == countryData.Name);
            if (country is null)
                context.Set<CountryEntity>().Add(new CountryEntity(default, countryData.Name));
        }
        context.SaveChanges();
    }
}
