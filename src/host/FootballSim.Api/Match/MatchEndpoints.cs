using ProjectFootballSim.Api.Teams;
using ProjectFootballSim.Match.Application.Common.Dtos;
using ProjectFootballSim.Match.Application.Features.ExtraTime;
using ProjectFootballSim.Match.Application.Features.Penalty;
using ProjectFootballSim.Match.Application.Features.RegularTime;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Api.Match;

internal static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        app.MapGet("/api/teams", async (TeamStore teamStore, CancellationToken cancellationToken) => {
            var teams = await teamStore.GetAllAsync(cancellationToken).ConfigureAwait(false);
            return Results.Ok(teams);
        });

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

            var homeTeam = new MatchTeamDto
            {
                Id = homePair.Id,
                Attack = homePair.Attack,
                Defence = homePair.Defence,
                Midfield = homePair.Midfield
            };

            var awayTeam = new MatchTeamDto
            {
                Id = awayPair.Id,
                Attack = awayPair.Attack,
                Defence = awayPair.Defence,
                Midfield = awayPair.Midfield
            };

            // Regular time
            var rtScore = regularTime.Play(homeTeam, awayTeam, settings);
            ScoreDto? etScore = null;
            ScoreDto? penScore = null;

            int finalHome = rtScore.HomeScore;
            int finalAway = rtScore.AwayScore;

            // Extra time if draw
            if (rtScore.HomeScore == rtScore.AwayScore)
            {
                var et = extraTime.Play(homeTeam, awayTeam, settings);
                etScore = new ScoreDto(et.HomeScore, et.AwayScore);
                finalHome += et.HomeScore;
                finalAway += et.AwayScore;

                // Penalties if still drawn
                if (finalHome == finalAway)
                {
                    var pen = PenaltySimulator.Play(homeTeam, awayTeam);
                    penScore = new ScoreDto(pen.HomeScore, pen.AwayScore);
                }
            }

            string winner = finalHome > finalAway
                ? homePair.Name
                : finalAway > finalHome
                    ? awayPair.Name
                    : penScore is not null && penScore.HomeScore != penScore.AwayScore
                        ? penScore.HomeScore > penScore.AwayScore
                            ? homePair.Name
                            : awayPair.Name
                        : "Draw";

            var result = new MatchResultResponse(
                homePair,
                awayPair,
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
