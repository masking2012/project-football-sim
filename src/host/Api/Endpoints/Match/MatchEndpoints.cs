using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Features.ExtraTime;
using ProjectFootballSim.Matches.Application.Features.Penalty;
using ProjectFootballSim.Matches.Application.Features.RegularTime;
using ProjectFootballSim.Teams.Application.Features.GetTeamById;
using System.Globalization;

namespace ProjectFootballSim.Api.Endpoints.Match;

internal static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        app.MapPost("/api/matches/simulate", async (
            SimulateMatchRequest req,
            RegularTimeSimulator regularTime,
            ExtraTimeSimulator extraTime,
            GetTeamByIdQuery getTeamByIdQuery) =>
        {
            int homeTeamId = Convert.ToInt32(req.HomeTeamId, CultureInfo.InvariantCulture);
            int awayTeamId = Convert.ToInt32(req.AwayTeamId, CultureInfo.InvariantCulture);

            var homePair = await getTeamByIdQuery.HandleAsync(homeTeamId, CancellationToken.None).ConfigureAwait(false);
            var awayPair = await getTeamByIdQuery.HandleAsync(awayTeamId, CancellationToken.None).ConfigureAwait(false);

            if (homePair is null)
                return Results.BadRequest($"Home team '{req.HomeTeamId}' not found.");
            if (awayPair is null)
                return Results.BadRequest($"Away team '{req.AwayTeamId}' not found.");
            if (req.HomeTeamId == req.AwayTeamId)
                return Results.BadRequest("Home and away teams must be different.");

            var settings = new MatchSettingsDto(HasHomeAdvantage: req.HasHomeAdvantage);

            var homeTeam = new MatchTeamDto
            (
                Id: Convert.ToInt32(homePair.Id, CultureInfo.InvariantCulture),
                Attack: homePair.Attack,
                Defence: homePair.Defence,
                Midfield: homePair.Midfield
            );

            var awayTeam = new MatchTeamDto
            (
                Id: Convert.ToInt32(awayPair.Id, CultureInfo.InvariantCulture),
                Attack: awayPair.Attack,
                Defence: awayPair.Defence,
                Midfield: awayPair.Midfield
            );

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
                new TeamDto(Id: homePair.Id.ToString(CultureInfo.InvariantCulture), Name: homePair.Name, Attack: homePair.Attack, Defence: homePair.Defence, Midfield: homePair.Midfield),
                new TeamDto(Id: awayPair.Id.ToString(CultureInfo.InvariantCulture), Name: awayPair.Name, Attack: awayPair.Attack, Defence: awayPair.Defence, Midfield: awayPair.Midfield),
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
