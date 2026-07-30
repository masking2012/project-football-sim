using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Endpoints.Match;
using ProjectFootballSim.Team.Application.Features.GetTeamsByCountry;
using System.Globalization;

namespace ProjectFootballSim.Api.Endpoints.Team;

internal static class TeamEndpoints
{
    public static void MapTeamEndpoints(this WebApplication app)
    {
        app.MapGet("/api/teams",
            async ([FromQuery] int countryId, GetTeamsByCountryFeature getTeamsByCountryFeature, CancellationToken cancellationToken) =>
            {
                var teamsByCountry = await getTeamsByCountryFeature.HandleAsync(countryId, cancellationToken).ConfigureAwait(false);
                var predefined = teamsByCountry.Select(t => new TeamDto(t.Id.ToString(CultureInfo.InvariantCulture), t.Name, t.Attack, t.Defence, t.Midfield)).ToList();
                return Results.Ok(predefined);
            });
    }
}
