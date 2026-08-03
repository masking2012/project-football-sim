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
            async ([FromQuery] int countryId, GetTeamsByCountryQueryHandler queryHandler, CancellationToken cancellationToken) =>
            {
                var teamsByCountry = await queryHandler.HandleAsync(countryId, cancellationToken).ConfigureAwait(false);
                var response = teamsByCountry
                    .Select(t => new TeamResponse(
                        Id: t.Id.ToString(CultureInfo.InvariantCulture),
                        Name: t.Name,
                        Attack: t.Attack,
                        Defence: t.Defence,
                        Midfield: t.Midfield)).ToList();
                return Results.Ok(response);
            }).RequireAuthorization();
    }
}
