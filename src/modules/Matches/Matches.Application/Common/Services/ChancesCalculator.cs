using ProjectFootballSim.Common.Features;
using ProjectFootballSim.Matches.Domain.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Common.Services;

internal sealed class ChancesCalculator : IChancesCalculator
{
    public int Calculate(MatchTeam attacking, MatchTeam defending,
        Possession attackingTeamPossession, AdvantageRatio attackingAdvantageRatio,
        GoalChancesSettings goalChancesSettings)
    {
        // Base chances on possession (more possession = more chances)
        double baseChances = attackingTeamPossession.Value * goalChancesSettings.BaseNumber;

        double attackPower =
            attacking.Attack * attackingAdvantageRatio.Value +
            attacking.Midfield * 0.25;

        double defencePower =
            defending.Defence +
            defending.Midfield * 0.25;
        double strengthRatio = Math.Sqrt(attackPower / Math.Max(defencePower, 1));

        double expectedChances = baseChances * strengthRatio;

        // Add randomness
        expectedChances *= CryptoRandom.NextDouble() * 0.4 + 0.8;

        expectedChances = Math.Clamp(expectedChances, goalChancesSettings.Minimum, goalChancesSettings.Maximum);

        return CryptoRandom.NextPoisson(expectedChances);
    }
}
