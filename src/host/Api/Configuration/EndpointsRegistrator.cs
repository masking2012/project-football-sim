using ProjectFootballSim.Api.Endpoints.Country;
using ProjectFootballSim.Api.Endpoints.Match;
using ProjectFootballSim.Api.Endpoints.Team;

namespace ProjectFootballSim.Api.Configuration;

internal static class EndpointsRegistrator
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        app.MapCountryEndpoints();
        app.MapTeamEndpoints();
        app.MapMatchEndpoints();
    }
}
