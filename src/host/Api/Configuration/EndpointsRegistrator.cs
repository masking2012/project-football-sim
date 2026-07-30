using ProjectFootballSim.Api.Endpoints.Countries;
using ProjectFootballSim.Api.Endpoints.Match;
using ProjectFootballSim.Api.Endpoints.Team;

namespace ProjectFootballSim.Api.Configuration;

internal static class EndpointsRegistrator
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        app.MapCountriesEndpoints();
        app.MapTeamsEndpoints();
        app.MapMatchEndpoints();
    }
}
