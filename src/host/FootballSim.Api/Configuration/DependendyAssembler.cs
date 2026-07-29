using ProjectFootballSim.FootballSim.Api.Endpoints.Team;
using ProjectFootballSim.Location.Application;
using ProjectFootballSim.Location.Infrastructure;
using ProjectFootballSim.Match.Application;
using ProjectFootballSim.Team.Application;
using ProjectFootballSim.Team.Infrastructure;

namespace ProjectFootballSim.FootballSim.Api.Configuration;

internal static class DependendyAssembler
{
    public static void AddAllDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddLocationInfrastructure(builder.Configuration, "LocationAzureSql");
        builder.Services.AddLocationApplication();

        builder.Services.AddTeamInfrastructure(builder.Configuration, "TeamAzureSql");
        builder.Services.AddTeamApplication();

        builder.Services.AddMatchApplication();
        builder.Services.AddScoped<TeamStore>();
    }
}
