using ProjectFootballSim.Locations.Application;
using ProjectFootballSim.Locations.Infrastructure;
using ProjectFootballSim.Matches.Application;
using ProjectFootballSim.Team.Application;
using ProjectFootballSim.Team.Infrastructure;

namespace ProjectFootballSim.Api.Configuration;

internal static class DependencyAssembler
{
    public static void AddAllDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddLocationsInfrastructure(builder.Configuration, "LocationsAzureSql");
        builder.Services.AddLocationsApplication();

        builder.Services.AddTeamInfrastructure(builder.Configuration, "TeamsAzureSql");
        builder.Services.AddTeamApplication();

        builder.Services.AddMatchesApplication();
    }
}
