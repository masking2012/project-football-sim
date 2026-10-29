using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Features.ExtraTime;
using ProjectFootballSim.Matches.Application.Features.Penalty;
using ProjectFootballSim.Matches.Application.Features.RegularTime;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;
using System.Globalization;

namespace ProjectFootballSim.Api.Endpoints.Match;

internal static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        app.MapPost("/api/matches/simulate", async (
            SimulateMatchRequest req,
            SimulateRegularTimeCommandHandler regularTime,
            SimulateExtraTimeCommandHandler extraTime,
            SimulatePenaltyShootoutCommandHandler penaltyShootout,
            IGetTeamsByIdsQueryHandler getTeamsByIdsQuery,
            CancellationToken cancellationToken) =>
        {
            int homeTeamId = Convert.ToInt32(req.HomeTeamId, CultureInfo.InvariantCulture);
            int awayTeamId = Convert.ToInt32(req.AwayTeamId, CultureInfo.InvariantCulture);
            if (homeTeamId == awayTeamId)
                return Results.BadRequest("Home and away teams must be different.");

            IReadOnlyDictionary<int, TeamByIdsDto> teams;
            try
            {
                teams = await getTeamsByIdsQuery.HandleAsync([homeTeamId, awayTeamId], cancellationToken).ConfigureAwait(false);
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(exception.Message);
            }

            if (!teams.TryGetValue(homeTeamId, out var homePair))
                return Results.BadRequest($"Home team '{req.HomeTeamId}' not found.");
            if (!teams.TryGetValue(awayTeamId, out var awayPair))
                return Results.BadRequest($"Away team '{req.AwayTeamId}' not found.");

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
            var simulateRegularTimeCommand = new SimulateRegularTimeCommand(homeTeam, awayTeam, settings);
            var rtScore = regularTime.Handle(simulateRegularTimeCommand);
            ScoreResponse? etScore = null;
            ScoreResponse? penScore = null;

            int finalHome = rtScore.HomeScore;
            int finalAway = rtScore.AwayScore;

            // Extra time if draw
            if (rtScore.HomeScore == rtScore.AwayScore)
            {
                var extraTimeCommand = new SimulateExtraTimeCommand(homeTeam, awayTeam, settings);
                var et = extraTime.Handle(extraTimeCommand);
                etScore = new ScoreResponse(et.HomeScore, et.AwayScore);
                finalHome += et.HomeScore;
                finalAway += et.AwayScore;

                // Penalties if still drawn
                if (finalHome == finalAway)
                {
                    var penaltyCommand = new SimulatePenaltyShootoutCommand(homeTeam, awayTeam);
                    var pen = penaltyShootout.Handle(penaltyCommand);
                    penScore = new ScoreResponse(pen.HomeScore, pen.AwayScore);
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
                new TeamResponse(Id: homePair.Id.ToString(CultureInfo.InvariantCulture), Name: homePair.Name, Attack: homePair.Attack, Defence: homePair.Defence, Midfield: homePair.Midfield),
                new TeamResponse(Id: awayPair.Id.ToString(CultureInfo.InvariantCulture), Name: awayPair.Name, Attack: awayPair.Attack, Defence: awayPair.Defence, Midfield: awayPair.Midfield),
                new ScoreResponse(rtScore.HomeScore, rtScore.AwayScore),
                etScore,
                penScore,
                new ScoreResponse(finalHome, finalAway),
                winner
            );

            return Results.Ok(result);
        }).RequireAuthorization();
    }
}
