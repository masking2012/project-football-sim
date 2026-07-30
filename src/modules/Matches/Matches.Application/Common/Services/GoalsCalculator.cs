using ProjectFootballSim.Common.Features;
using ProjectFootballSim.Matches.Domain.Services;

namespace ProjectFootballSim.Matches.Application.Common.Services;

internal sealed class GoalsCalculator : IGoalsCalculator
{
    public int Calculate(int attack, int opponentDefense, int chances)
    {
        double ratio = attack / (double)(attack + opponentDefense);

        double conversionRate =
            Math.Clamp(0.05 + ratio * 0.15, 0.05, 0.20);

        int goals = 0;

        for (int i = 0; i < chances; i++)
        {
            // Slight variation in chance quality.
            double quality =
                conversionRate *
                (CryptoRandom.NextDouble() * 0.3 + 0.85);

            if (CryptoRandom.NextDouble() < quality)
                goals++;
        }

        return goals;
    }
}
