using ProjectFootballSim.Locations.Application;
using ProjectFootballSim.Locations.Infrastructure;
using ProjectFootballSim.Matches.Application;
using ProjectFootballSim.Teams.Application;
using ProjectFootballSim.Teams.Infrastructure;

namespace ProjectFootballSim.Api.Configuration;

internal static class DependencyAssembler
{
    public static void AddAllDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddLocationsInfrastructure(builder.Configuration, "LocationsAzureSql");
        builder.Services.AddLocationsApplication();

        builder.Services.AddTeamsInfrastructure(builder.Configuration, "TeamsAzureSql");
        builder.Services.AddTeamsApplication();

        builder.Services.AddMatchesApplication();
    }
}
