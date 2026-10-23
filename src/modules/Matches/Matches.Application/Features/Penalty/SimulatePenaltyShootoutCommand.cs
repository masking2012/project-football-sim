using Microsoft.Extensions.Logging;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;
using ProjectFootballSim.Shared.Features.Random;

namespace ProjectFootballSim.Matches.Application.Features.Penalty;

public sealed class SimulatePenaltyShootoutCommand(
    ILogger<SimulatePenaltyShootoutCommand> logger,
    IRandomNumberGenerator random)
{
    public ScoreResultDto Handle(MatchTeamDto homeDto, MatchTeamDto awayDto)
    {
        ArgumentNullException.ThrowIfNull(homeDto);
        ArgumentNullException.ThrowIfNull(awayDto);

        MatchTeam home = TeamMapper.Map(homeDto);
        MatchTeam away = TeamMapper.Map(awayDto);

        int homeScore = 0;
        int awayScore = 0;

        // Calculate penalty conversion rates based on attack (composure) and defence (goalkeeper)
        double homeConversionRate = CalculatePenaltyConversionRate(home.Attack, away.Defence);
        double awayConversionRate = CalculatePenaltyConversionRate(away.Attack, home.Defence);

        // First 5 penalties each
        for (int i = 0; i < 5; i++)
        {
            // Home penalty
            if (random.NextDouble() < homeConversionRate)
                homeScore++;

            int remainingAwayKicks = 5 - i;
            if (homeScore - awayScore > remainingAwayKicks)
                break;

            // Away penalty
            if (random.NextDouble() < awayConversionRate)
                awayScore++;

            // Early exit if one team can't catch up
            int remainingHomeKicks = 4 - i;
            if (awayScore - homeScore > remainingHomeKicks)
                break;
        }

        // Sudden death if still tied
        while (homeScore == awayScore)
        {
            bool homeScores = random.NextDouble() < homeConversionRate;
            bool awayScores = random.NextDouble() < awayConversionRate;

            if (homeScores) homeScore++;
            if (awayScores) awayScore++;

            // If one scored and other didn't, we have a winner
            if (homeScores != awayScores)
                break;
        }

        Log.PenaltyShootoutResult(logger, homeDto.Id, homeScore, awayDto.Id, awayScore);

        return new ScoreResultDto(homeScore, awayScore);
    }

    private static double CalculatePenaltyConversionRate(double attack, double opponentDefence)
    {
        // Base penalty conversion rate is around 75-80%
        double baseRate = 0.77;

        // Attack rating affects composure (max +8%)
        double attackBonus = (attack / 100.0) * 0.08;

        // Opponent defence affects goalkeeper quality (max -8%)
        double defenceReduction = (opponentDefence / 100.0) * 0.08;

        double conversionRate = baseRate + attackBonus - defenceReduction;

        return Math.Clamp(conversionRate, 0.65, 0.90);
    }
}
