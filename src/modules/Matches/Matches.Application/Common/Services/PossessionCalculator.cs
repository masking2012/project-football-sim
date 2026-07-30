using ProjectFootballSim.Common.Features;
using ProjectFootballSim.Matches.Domain.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Common.Services;

internal sealed class PossessionCalculator : IPossessionCalculator
{
    public Possession Calculate(int homeMidfield, int awayMidfield)
    {
        double difference = homeMidfield - awayMidfield;

        // Convert midfield difference into a possession probability.
        // For example, increase of 0.003 → 0.006 impacts on how strongly midfield differences affect possession.
        double possession = 0.5 + difference * 0.006;

        // Small match-to-match variation (±2%).
        // For example, reducing 0.06 → 0.04 makes results less noisy and more consistent from match to match.
        possession += (CryptoRandom.NextDouble() - 0.5) * 0.04;

        return new Possession(Math.Clamp(possession, IPossessionCalculator.MinPossession, IPossessionCalculator.MaxPossession));
    }
}
