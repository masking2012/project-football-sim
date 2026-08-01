using ProjectFootballSim.Seasons.Application.Features.GetCurrentSeason;
using ProjectFootballSim.Seasons.Application.Features.GetSeasonDefinition;
using ProjectFootballSim.Seasons.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace ProjectFootballSim.Api.Endpoints.Seasons;

internal static class SeasonsEndpoints
{
    public static void MapSeasonsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/seasons", async (
            CreateNextPlayerSeasonCommandHandler commandHandler,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out Guid userId))
                return Results.Unauthorized();

            await commandHandler.HandleAsync(new CreatePlayerSeasonCommand(userId, DateTime.UtcNow), cancellationToken).ConfigureAwait(false);
            return Results.StatusCode(201);

        }).RequireAuthorization();

        app.MapGet("/api/seasons/current", async (
            GetCurrentPlayerSeasonQueryHandler queryHandler,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out Guid userId))
                return Results.Unauthorized();

            CurrentSeasonDto? currentSeason = await queryHandler.HandleAsync(userId, cancellationToken).ConfigureAwait(false);
            if (currentSeason is null)
                return Results.NotFound();

            return Results.Ok(
                new CurrentSeasonResponse(currentSeason.Id, currentSeason.StartDate, currentSeason.EndDate));

        }).RequireAuthorization();
    }
}
