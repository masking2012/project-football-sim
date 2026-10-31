using Microsoft.AspNetCore.Mvc;
using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.Calendar.Contracts.Features.AdvanceCalendarDay;
using ProjectFootballSim.Calendar.Contracts.Features.GetCalendarDay;
using ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;
using System.Security.Claims;

namespace ProjectFootballSim.Api.Endpoints.Calendar;

internal static class CalendarEndpoints
{
    public static void MapCalendarEndpoints(this WebApplication app)
    {
        app.MapGet("/api/games/{gameId}/calendar/day", async (
            [FromRoute] Guid gameId,
            IGetCalendarDayQueryHandler getCalendarDayQueryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var result = await getCalendarDayQueryHandler
                .HandleAsync(new GetCalendarDayQuery(UserId: userId, GameId: gameId, Date: null), cancellationToken)
                .ConfigureAwait(false);

            //TODO: change LeagueMatchResponse to Dto (?)
            return Results.Ok(new CalendarDayResponse(
                Date: result.Date,
                DayStatus: result.DayStatus,
                LeagueMatches: result.LeagueMatches.Select(m => new LeagueMatchResponse(
                    Id: m.Id,
                    HomeTeamId: m.HomeTeamId,
                    HomeTeamName: m.HomeTeamName,
                    AwayTeamId: m.AwayTeamId,
                    AwayTeamName: m.AwayTeamName,
                    HomeTeamScore: m.HomeTeamScore,
                    AwayTeamScore: m.AwayTeamScore,
                    Round: m.LeagueRound,
                    LeagueName: m.LeagueName,
                    LeagueId: m.LeagueVersionId,
                    CountryId: m.CountryId,
                    CountryName: m.CountryName,
                    Order: m.LeagueOrder
                ))
            ));
        }).RequireAuthorization();

        app.MapPost("/api/games/{gameId}/calendar/simulate", async (
            [FromRoute] Guid gameId,
            ISimulateCalendarDayCommandHandler simulateCalendarDayCommandHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var command = new SimulateCalendarDayCommand(UserId: userId, GameId: gameId);
            await simulateCalendarDayCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return Results.NoContent();
        }).RequireAuthorization();

        app.MapPost("/api/games/{gameId}/calendar/proceed", async (
            [FromRoute] Guid gameId,
            IAdvanceCalendarDayCommandHandler advanceCalendarDayCommandHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            var command = new AdvanceCalendarDayCommand(UserId: userId, GameId: gameId);
            await advanceCalendarDayCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return Results.NoContent();
        }).RequireAuthorization();
    }
}
