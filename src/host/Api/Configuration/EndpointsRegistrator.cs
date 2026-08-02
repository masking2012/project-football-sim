using ProjectFootballSim.Api.Endpoints.Countries;
using ProjectFootballSim.Api.Endpoints.Identities;
using ProjectFootballSim.Api.Endpoints.Match;
using ProjectFootballSim.Api.Endpoints.Seasons;
using ProjectFootballSim.Api.Endpoints.Team;

namespace ProjectFootballSim.Api.Configuration;

internal static class EndpointsRegistrator
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        app.MapIdentityEndpoints();
        app.MapCountriesEndpoints();
        app.MapTeamsEndpoints();
        app.MapMatchEndpoints();
        app.MapSeasonsEndpoints();
    }
}
