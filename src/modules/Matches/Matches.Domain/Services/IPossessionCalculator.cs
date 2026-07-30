using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Domain.Services;

public interface IPossessionCalculator
{
    const double MinPossession = 0.2;
    const double MaxPossession = 0.8;

    Possession Calculate(int homeMidfield, int awayMidfield);
}
