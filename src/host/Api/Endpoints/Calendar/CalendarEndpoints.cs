using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.Calendar.Application.Features.GetDayWithEvents;
using ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;
using ProjectFootballSim.Calendar.Application.Features.ProceedCalendar;
using ProjectFootballSim.Calendar.Application.Features.SimulateGameDay;
using ProjectFootballSim.Locations.Application.Features.GetCountries;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByIds;
using System.Security.Claims;

namespace ProjectFootballSim.Api.Endpoints.Calendar;

internal static class CalendarEndpoints
{
    public static void MapCalendarEndpoints(this WebApplication app)
    {
        app.MapGet("/api/games/{gameId}/events", async (
            [FromRoute] Guid gameId,
            GetDayWithEventsQueryHandler getEventsByDateQueryHandler,
            GetCountriesQueryHandler getCountriesQueryHandler,
            GetTeamsByIdsQueryHandler getTeamsByIdsQueryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var result = await getEventsByDateQueryHandler
                .HandleAsync(new GetDayWithEventsQuery(UserId: userId, GameId: gameId, Date: null), cancellationToken)
                .ConfigureAwait(false);
            var countries = await getCountriesQueryHandler.HandleAsync(cancellationToken).ConfigureAwait(false);
            var teams = await getTeamsByIdsQueryHandler
                .HandleAsync(result.MatchEvents.SelectMany(x => new[] { x.HomeTeamId, x.AwayTeamId }).Distinct().ToList(), cancellationToken)
                .ConfigureAwait(false);

            return Results.Ok(new DayDetailsResponse(
                Date: result.Date,
                DayState: result.DayState,
                MatchEvents: result.MatchEvents.Select(x => new MatchEventResponse(
                    Id: x.Id,
                    HomeTeamId: x.HomeTeamId,
                    HomeTeamName: teams[x.HomeTeamId].Name,
                    AwayTeamId: x.AwayTeamId,
                    AwayTeamName: teams[x.AwayTeamId].Name,
                    HomeTeamScore: x.HomeTeamScore,
                    AwayTeamScore: x.AwayTeamScore,
                    Round: x.Round,
                    LeagueName: x.LeagueName,
                    LeagueId: x.LeagueId,
                    CountryId: x.CountryId,
                    CountryName: countries.Single(c => c.Id == x.CountryId).Name
                ))
            ));
        }).RequireAuthorization();

        app.MapPost("/api/games/{gameId}/calendar/simulate", async (
            [FromRoute] Guid gameId,
            SimulateGameDayCommandHandler simulateGameDayCommandHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var command = new SimulateGameDayCommand(UserId: userId, GameId: gameId);
            await simulateGameDayCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return Results.Ok();
        }).RequireAuthorization();

        app.MapGet("/api/games/{gameId}/calendar/proceed", async (
            [FromRoute] Guid gameId,
            ProceedCalendarCommandHandler proceedCalendarCommandHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var command = new ProceedCalendarCommand(UserId: userId, GameId: gameId);
            await proceedCalendarCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return Results.Ok();
        }).RequireAuthorization();
    }
}
