using ProjectFootballSim.FootballSim.Api.Endpoints.Country;
using ProjectFootballSim.FootballSim.Api.Endpoints.Match;
using ProjectFootballSim.FootballSim.Api.Endpoints.Team;

namespace ProjectFootballSim.FootballSim.Api.Configuration;

internal static class EndpointsRegistrator
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        app.MapCountryEndpoints();
        app.MapTeamEndpoints();
        app.MapMatchEndpoints();
    }
}
