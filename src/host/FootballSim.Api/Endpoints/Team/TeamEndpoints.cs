using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.FootballSim.Api.Endpoints.Match;
using ProjectFootballSim.Team.Application.Features.GetTeamsByCountry;
using System.Globalization;

namespace ProjectFootballSim.FootballSim.Api.Endpoints.Team;

internal static class TeamEndpoints
{
    public static void MapTeamEndpoints(this WebApplication app)
    {
        app.MapGet("/api/teams",
            async ([FromQuery] string countryId, GetTeamsByCountryFeature getTeamsByCountryFeature, CancellationToken cancellationToken) =>
            {
                int parsedCountryId = Convert.ToInt32(countryId, CultureInfo.InvariantCulture);

                var teamsByCountry = await getTeamsByCountryFeature.HandleAsync(parsedCountryId, cancellationToken).ConfigureAwait(false);
                var predefined = teamsByCountry.Select(t => new TeamDto(t.Id.ToString(CultureInfo.InvariantCulture), t.Name, t.Attack, t.Defence, t.Midfield)).ToList();
                return Results.Ok(predefined);
            });
    }
}
