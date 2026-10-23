namespace ProjectFootballSim.Shared.Features.Random;

public interface IRandomNumberGenerator
{
    double NextDouble();

    int NextPoisson(double lambda);
}
