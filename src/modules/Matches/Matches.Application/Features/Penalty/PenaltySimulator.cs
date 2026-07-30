using ProjectFootballSim.Common.Features;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Features.Penalty;

public sealed class PenaltySimulator
{
    public static ScoreResultDto Play(MatchTeamDto homeDto, MatchTeamDto awayDto)
    {
        ArgumentNullException.ThrowIfNull(homeDto);
        ArgumentNullException.ThrowIfNull(awayDto);

        MatchTeam home = TeamMapper.Map(homeDto);
        MatchTeam away = TeamMapper.Map(awayDto);

        int homeScore = 0;
        int awayScore = 0;

        // Calculate penalty conversion rates based on attack (composure) and defense (goalkeeper)
        double homeConversionRate = CalculatePenaltyConversionRate(home.Attack, away.Defence);
        double awayConversionRate = CalculatePenaltyConversionRate(away.Attack, home.Defence);

        // First 5 penalties each
        for (int i = 0; i < 5; i++)
        {
            // Home penalty
            if (CryptoRandom.NextDouble() < homeConversionRate)
                homeScore++;

            // Away penalty
            if (CryptoRandom.NextDouble() < awayConversionRate)
                awayScore++;

            // Early exit if one team can't catch up
            int remaining = 5 - (i + 1);
            if (homeScore - awayScore > remaining)
                break;
            if (awayScore - homeScore > remaining)
                break;
        }

        // Sudden death if still tied
        while (homeScore == awayScore)
        {
            bool homeScores = CryptoRandom.NextDouble() < homeConversionRate;
            bool awayScores = CryptoRandom.NextDouble() < awayConversionRate;

            if (homeScores) homeScore++;
            if (awayScores) awayScore++;

            // If one scored and other didn't, we have a winner
            if (homeScores != awayScores)
                break;
        }

        return new ScoreResultDto(homeScore, awayScore);
    }

    private static double CalculatePenaltyConversionRate(double attack, double opponentDefense)
    {
        // Base penalty conversion rate is around 75-80%
        double baseRate = 0.77;

        // Attack rating affects composure (max +8%)
        double attackBonus = (attack / 100.0) * 0.08;

        // Opponent defense affects goalkeeper quality (max -8%)
        double defenseReduction = (opponentDefense / 100.0) * 0.08;

        double conversionRate = baseRate + attackBonus - defenseReduction;

        return Math.Clamp(conversionRate, 0.65, 0.90);
    }
}
