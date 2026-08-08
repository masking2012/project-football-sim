using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;
using ProjectFootballSim.Locations.Application.Features.GetCountries;
using System.Security.Claims;

namespace ProjectFootballSim.Api.Endpoints.Calendar;

internal static class CalendarEndpoints
{
    public static void MapCalendarEndpoints(this WebApplication app)
    {
        app.MapGet("/api/games/{gameId}/events", async (
            [FromRoute] Guid gameId,
            GetEventsByDateQueryHandler getEventsByDateQueryHandler,
            GetCountriesQueryHandler getCountriesQueryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var events = await getEventsByDateQueryHandler
                .HandleAsync(new GetEventsByDateQuery(UserId: userId, GameId: gameId, Date: null), cancellationToken)
                .ConfigureAwait(false);
            var countries = await getCountriesQueryHandler.HandleAsync(cancellationToken).ConfigureAwait(false);

            return Results.Ok(events.Select(x => new MatchEventResponse(
                Id: x.Id,
                HomeTeamId: x.HomeTeamId,
                AwayTeamId: x.AwayTeamId,
                HomeTeamScore: x.HomeTeamScore,
                AwayTeamScore: x.AwayTeamScore,
                Round: x.Round,
                LeagueName: x.LeagueName,
                LeagueId: x.LeagueId,
                CountryId: x.CountryId,
                CountryName: countries.Single(c => c.Id == x.CountryId).Name
            )));
        }).RequireAuthorization();
    }
}
