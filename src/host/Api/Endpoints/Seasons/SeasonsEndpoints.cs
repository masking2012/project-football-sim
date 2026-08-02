using ProjectFootballSim.Api.Extensions;
using ProjectFootballSim.Seasons.Application.Features.CreateNextPlayerSeason;
using ProjectFootballSim.Seasons.Application.Features.GetCurrentSeason;
using System.Security.Claims;

namespace ProjectFootballSim.Api.Endpoints.Seasons;

internal static class SeasonsEndpoints
{
    public static void MapSeasonsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/seasons", async (
            StartSeasonRequest request,
            CreateNextPlayerSeasonCommandHandler commandHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            await commandHandler.HandleAsync(new CreatePlayerSeasonCommand(request.GameId, userId, DateTime.UtcNow), cancellationToken).ConfigureAwait(false);
            return Results.StatusCode(201);

        }).RequireAuthorization();

        app.MapGet("/api/seasons/current", async (
            GetCurrentPlayerSeasonQueryHandler queryHandler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (!user.TryGetUserId(out var userId))
                return Results.Unauthorized();

            CurrentSeasonDto? currentSeason = await queryHandler.HandleAsync(userId, cancellationToken).ConfigureAwait(false);
            if (currentSeason is null)
                return Results.NotFound();

            return Results.Ok(
                new CurrentSeasonResponse(currentSeason.Id, currentSeason.StartDate, currentSeason.EndDate));

        }).RequireAuthorization();
    }
}
