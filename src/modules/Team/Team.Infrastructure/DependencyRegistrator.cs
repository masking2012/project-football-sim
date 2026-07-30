using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Common.Data.Entities.Countries;
using ProjectFootballSim.Common.Data.Entities.Teams;
using ProjectFootballSim.Team.Domain.Entities;
using ProjectFootballSim.Team.Domain.ValueObjects;
using ProjectFootballSim.Team.Infrastructure.Database;

namespace ProjectFootballSim.Team.Infrastructure;

public static class DependencyRegistrator
{
    public static IServiceCollection AddTeamInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringSectionName)
    {
        services.AddDbContext<TeamDbContext>(options =>
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
        var countries = CountryDataProvider.GetAll();

        foreach (var countryData in countries)
        {
            foreach (var teamData in TeamDataProvider.GetAll(countryData.Id))
            {
                var team = context.Set<TeamEntity>().SingleOrDefault(c => c.Id == teamData.Id);
                if (team is null)
                    context.Set<TeamEntity>().Add(
                        new TeamEntity(
                            id: teamData.Id,
                            name: teamData.Name,
                            attack: new AttributeValue(teamData.Attack),
                            midfield: new AttributeValue(teamData.Midfield),
                            defence: new AttributeValue(teamData.Defence),
                            countryId: teamData.CountryId));
                else
                    team.Update(
                        name: teamData.Name,
                        attack: new AttributeValue(teamData.Attack),
                        midfield: new AttributeValue(teamData.Midfield),
                        defence: new AttributeValue(teamData.Defence),
                        countryId: teamData.CountryId);
            }
        }

        context.SaveChanges();
    }
}
