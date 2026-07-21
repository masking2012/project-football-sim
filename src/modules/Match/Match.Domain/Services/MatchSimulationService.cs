using Common.Features;
using ProjectFootballSim.Match.Domain.ValueObjects;
using ProjectFootballSim.ValueObjects;

namespace ProjectFootballSim.Services;

public sealed class MatchSimulationService
{
    private readonly double _homeAdvantage = 1.1;
    private readonly double _extraTimeMultiplier = 0.3; // 30% of regular time intensity

    public MatchResult SimulateMatch(Team home, Team away, MatchSettings matchSettings)
    {
        ArgumentNullException.ThrowIfNull(home);
        ArgumentNullException.ThrowIfNull(away);
        ArgumentNullException.ThrowIfNull(matchSettings);

        (int homeGoals, int awayGoals) = CalculateResult(home, away, matchSettings, isExtraTime: false);

        if (matchSettings.HasExtraTime && homeGoals == awayGoals)
        {
            // Simulate extra time
            (int homeExtraGoals, int awayExtraGoals) = CalculateResult(home, away, matchSettings, isExtraTime: true);

            if (matchSettings.HasPenaltyShootout && homeExtraGoals == awayExtraGoals)
            {
                // Simulate penalty shootout
                (int homePenaltyGoals, int awayPenaltyGoals) = PenaltySimulationService.SimulatePenalties(home, away);
            }

            return new MatchResult(homeGoals, awayGoals, homeExtraGoals, awayExtraGoals);
        }

        if (matchSettings.HasPenaltyShootout && homeGoals == awayGoals)
        {
            // Simulate penalty shootout
            (int homePenaltyGoals, int awayPenaltyGoals) = PenaltySimulationService.SimulatePenalties(home, away);
        }

        return new MatchResult(homeGoals, awayGoals);
    }

    private (int, int) CalculateResult(Team home, Team away, MatchSettings matchSettings, bool isExtraTime)
    {
        // Calculate possession based on midfield strength
        double possessionRatio = CalculatePossession(home.Midfield, away.Midfield);
        //result.HomePossession = possessionRatio;

        // Calculate number of attacking chances based on possession and attack/defense matchup
        int homeChances = CalculateChances(home, away, possessionRatio, advantageForAttacking: matchSettings.HasHomeAdvantage);
        int awayChances = CalculateChances(away, home, 1 - possessionRatio, advantageForAttacking: false);

        // Simulate extra time (30 minutes = 1/3 of regular time)
        if (isExtraTime)
        {
            homeChances = (int)(homeChances * _extraTimeMultiplier);
            awayChances = (int)(awayChances * _extraTimeMultiplier);
        }

        // Convert chances to goals
        int homeGoals = CalculateGoals(home.Attack, away.Defence, homeChances);
        int awayGoals = CalculateGoals(away.Attack, home.Defence, awayChances);

        return (homeGoals, awayGoals);
    }

    private static double CalculatePossession(int homeMid, int awayMid)
    {
        // Normalize to prevent extreme possession values
        double total = homeMid + awayMid;
        if (total == 0) return 0.5;

        double basePossession = homeMid / total;

        // Add some randomness (±10%)
        double randomFactor = (CryptoRandom.NextDouble() - 0.5) * 0.2;
        return Math.Clamp(basePossession + randomFactor, 0.3, 0.7);
    }

    private int CalculateChances(Team attacking, Team defending, double possession, bool advantageForAttacking)
    {
        // Base chances on possession (more possession = more chances)
        double baseChances = possession * 20; // 0-14 base chances

        double attack = advantageForAttacking ? attacking.Attack * _homeAdvantage : attacking.Attack;
        double defence = advantageForAttacking ? defending.Defence : defending.Defence * _homeAdvantage;

        // Modify by attack vs defense strength
        double attackPower = attack + attacking.Midfield / 2.0;
        double defensePower = defending.Defence + defending.Midfield / 2.0;

        double strengthRatio = attackPower / Math.Max(defensePower, 1);
        double adjustedChances = baseChances * strengthRatio;

        // Add randomness
        double randomFactor = CryptoRandom.NextDouble() * 0.4 + 0.8; // 0.8 to 1.2
        adjustedChances *= randomFactor;


        //// Add randomness - THIS IS KEY PARAMETER #2: ChancesRandomness
        ///ChancesRandomness: 0.4 
        //double randomFactor = _random.NextDouble() * ChancesRandomness + (1 - ChancesRandomness / 2);
        //adjustedChances *= randomFactor;

        return Math.Max(1, (int)Math.Round(adjustedChances));
    }

    private static int CalculateGoals(int attack, int defense, int chances)
    {
        int goals = 0;

        // Calculate conversion rate based on attack vs defense
        double attackPower = attack / 100.0;
        double defensePower = defense / 100.0;

        // Base conversion rate: stronger attack vs weaker defense = higher conversion
        // attack: Range: 0.20 to 0.30
        // defence: Range: 0.45 to 0.55
        double conversionRate = (attackPower * 0.3) * (1 - defensePower * 0.5);
        conversionRate = Math.Clamp(conversionRate, 0.05, 0.35);

        // Each chance has a probability to become a goal
        for (int i = 0; i < chances; i++)
        {
            if (CryptoRandom.NextDouble() < conversionRate)
            {
                goals++;
            }
        }

        return goals;
    }
}
