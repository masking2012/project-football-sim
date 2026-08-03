using ProjectFootballSim.Common.Features;
using ProjectFootballSim.Matches.Domain.Services;

namespace ProjectFootballSim.Matches.Application.Common.Services;

internal sealed class GoalsCalculator : IGoalsCalculator
{
    public int Calculate(int attack, int opponentDefence, int chances)
    {
        double difference = attack - opponentDefence;
        // Equal teams ~12%, stronger attacks convert noticeably better.
        double conversionRate = 0.12 + difference * 0.0025;
        conversionRate = Math.Clamp(conversionRate, 0.06, 0.28);

        // Many chances tend to be lower quality on average.
        conversionRate *= 1.0 - Math.Min(chances * 0.003, 0.12);

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
