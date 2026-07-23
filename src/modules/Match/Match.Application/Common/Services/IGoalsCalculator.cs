using ProjectFootballSim.Common.Features;

namespace ProjectFootballSim.Match.Application.Common.Services;

public interface IGoalsCalculator
{
    int Calculate(int attack, int opponentDefense, int chances);
}

internal sealed class GoalsCalculator : IGoalsCalculator
{
    public int Calculate(int attack, int opponentDefense, int chances)
    {
        double ratio = attack / (double)(attack + opponentDefense);

        double conversionRate =
            Math.Clamp(0.08 + ratio * 0.20, 0.08, 0.28);

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
