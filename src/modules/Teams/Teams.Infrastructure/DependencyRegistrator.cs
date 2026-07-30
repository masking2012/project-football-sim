using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Common.Data.Entities.Countries;
using ProjectFootballSim.Common.Data.Entities.Teams;
using ProjectFootballSim.Teams.Domain.Entities;
using ProjectFootballSim.Teams.Domain.ValueObjects;
using ProjectFootballSim.Teams.Infrastructure.Database;

namespace ProjectFootballSim.Teams.Infrastructure;

public static class DependencyRegistrator
{
    public static IServiceCollection AddTeamsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringSectionName)
    {
        services.AddDbContext<TeamsDbContext>(options =>
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
                var team = context.Set<Team>().SingleOrDefault(c => c.Id == teamData.Id);
                if (team is null)
                    context.Set<Team>().Add(
                        new Team(
                            id: teamData.Id,
                            name: teamData.Name,
                            attack: new TeamAttributeValue(teamData.Attack),
                            midfield: new TeamAttributeValue(teamData.Midfield),
                            defence: new TeamAttributeValue(teamData.Defence),
                            countryId: teamData.CountryId));
                else
                    team.Update(
                        name: teamData.Name,
                        attack: new TeamAttributeValue(teamData.Attack),
                        midfield: new TeamAttributeValue(teamData.Midfield),
                        defence: new TeamAttributeValue(teamData.Defence),
                        countryId: teamData.CountryId);
            }
        }

        context.SaveChanges();
    }
}
