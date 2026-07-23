using ProjectFootballSim.Common.Features;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Match.Application.Common.Services;

public interface IPossessionCalculator
{
    const double MinPossession = 0.2;
    const double MaxPossession = 0.8;

    Possession Calculate(int homeMidfield, int awayMidfield);
}

internal sealed class PossessionCalculator : IPossessionCalculator
{
    public Possession Calculate(int homeMidfield, int awayMidfield)
    {
        double difference = homeMidfield - awayMidfield;

        // Convert midfield difference into a possession probability.
        double possession = 0.5 + difference * 0.003;

        // Small match-to-match variation (±3%).
        possession += (CryptoRandom.NextDouble() - 0.5) * 0.06;

        return new Possession(Math.Clamp(possession, IPossessionCalculator.MinPossession, IPossessionCalculator.MaxPossession));
    }
}
