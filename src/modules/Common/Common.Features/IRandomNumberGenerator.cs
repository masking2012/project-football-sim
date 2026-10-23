namespace ProjectFootballSim.Common.Features;

public interface IRandomNumberGenerator
{
    double NextDouble();

    int NextPoisson(double lambda);
}
