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

        // Increase this to 0.35–0.45 for stronger teams to create more chances even with similar possession.
        double attackPower =
            attacking.Attack * attackingAdvantageRatio.Value +
            attacking.Midfield * 0.35;

        double defencePower =
            defending.Defence +
            defending.Midfield * 0.25;

        double ratio = attackPower / (attackPower + defencePower);
        double multiplier = 0.55 + ratio;
        double expectedChances = baseChances * multiplier;

        // Add randomness
        expectedChances *= CryptoRandom.NextDouble() * 0.2 + 0.9;

        expectedChances = Math.Clamp(expectedChances, goalChancesSettings.Minimum, goalChancesSettings.Maximum);

        return CryptoRandom.NextPoisson(expectedChances);
    }
}
