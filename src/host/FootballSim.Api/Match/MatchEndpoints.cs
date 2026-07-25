using ProjectFootballSim.Api.Teams;
using ProjectFootballSim.Match.Application.Features.ExtraTime;
using ProjectFootballSim.Match.Application.Features.Penalty;
using ProjectFootballSim.Match.Application.Features.RegularTime;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Api.Match;

internal static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        app.MapGet("/api/teams", () => Results.Ok(TeamStore.GetAll()));

        app.MapPost("/api/matches/simulate", (
            SimulateMatchRequest req,
            RegularTimeSimulator regularTime,
            ExtraTimeSimulator extraTime) =>
        {
            var homePair = TeamStore.FindById(req.HomeTeamId);
            var awayPair = TeamStore.FindById(req.AwayTeamId);

            if (homePair is null)
                return Results.BadRequest($"Home team '{req.HomeTeamId}' not found.");
            if (awayPair is null)
                return Results.BadRequest($"Away team '{req.AwayTeamId}' not found.");
            if (req.HomeTeamId == req.AwayTeamId)
                return Results.BadRequest("Home and away teams must be different.");

            var settings = new MatchSettings { HasHomeAdvantage = req.HasHomeAdvantage };

            // Regular time
            var rtScore = regularTime.Play(homePair.Value.Domain, awayPair.Value.Domain, settings);
            ScoreDto? etScore = null;
            ScoreDto? penScore = null;

            int finalHome = rtScore.HomeScore;
            int finalAway = rtScore.AwayScore;

            // Extra time if draw
            if (rtScore.HomeScore == rtScore.AwayScore)
            {
                var et = extraTime.Play(homePair.Value.Domain, awayPair.Value.Domain, settings);
                etScore = new ScoreDto(et.HomeScore, et.AwayScore);
                finalHome += et.HomeScore;
                finalAway += et.AwayScore;

                // Penalties if still drawn
                if (finalHome == finalAway)
                {
                    var pen = PenaltySimulator.Play(homePair.Value.Domain, awayPair.Value.Domain);
                    penScore = new ScoreDto(pen.HomeScore, pen.AwayScore);
                    finalHome = pen.HomeScore;
                    finalAway = pen.AwayScore;
                }
            }

            string winner = finalHome > finalAway
                ? homePair.Value.Dto.Name
                : finalAway > finalHome
                    ? awayPair.Value.Dto.Name
                    : "Draw";

            var result = new MatchResultResponse(
                homePair.Value.Dto,
                awayPair.Value.Dto,
                new ScoreDto(rtScore.HomeScore, rtScore.AwayScore),
                etScore,
                penScore,
                new ScoreDto(finalHome, finalAway),
                winner
            );

            return Results.Ok(result);
        });
    }
}
