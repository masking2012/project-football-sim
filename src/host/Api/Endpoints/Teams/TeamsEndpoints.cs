using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Endpoints.Match;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;
using System.Globalization;

namespace ProjectFootballSim.Api.Endpoints.Team;

internal static class TeamsEndpoints
{
    public static void MapTeamsEndpoints(this WebApplication app)
    {
        app.MapGet("/api/teams",
            async ([FromQuery] int countryId, GetTeamsByCountryQuery getTeamsByCountryQuery, CancellationToken cancellationToken) =>
            {
                var teamsByCountry = await getTeamsByCountryQuery.HandleAsync(countryId, cancellationToken).ConfigureAwait(false);
                var predefined = teamsByCountry.Select(t => new TeamResponse(t.Id.ToString(CultureInfo.InvariantCulture), t.Name, t.Attack, t.Defence, t.Midfield)).ToList();
                return Results.Ok(predefined);
            }).RequireAuthorization();
    }
}
