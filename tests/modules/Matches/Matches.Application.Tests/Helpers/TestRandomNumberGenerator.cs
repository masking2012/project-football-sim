using ProjectFootballSim.Shared.Features.Random;

namespace ProjectFootballSim.Matches.Application.Tests.Helpers;

internal sealed class TestRandomNumberGenerator : IRandomNumberGenerator
{
    private readonly Queue<double>? _values;
    private ulong _state;

    public TestRandomNumberGenerator(int seed)
    {
        _state = unchecked((ulong)seed);
        if (_state == 0)
            _state = 0x9E3779B97F4A7C15UL;
    }

    public TestRandomNumberGenerator(IEnumerable<double> values)
    {
        _values = new Queue<double>(values);
    }

    public double NextDouble()
    {
        if (_values is not null)
        {
            if (_values.TryDequeue(out double value))
                return value;

            throw new InvalidOperationException("No deterministic random values remain.");
        }

        _state ^= _state >> 12;
        _state ^= _state << 25;
        _state ^= _state >> 27;
        ulong sample = _state * 2685821657736338717UL;
        return (sample >> 11) * (1.0 / (1UL << 53));
    }

    public int NextPoisson(double lambda)
    {
        if (lambda <= 0)
            return 0;

        double limit = Math.Exp(-lambda);
        int k = 0;
        double product = 1.0;

        do
        {
            k++;
            product *= NextDouble();
        }
        while (product > limit);

        return k - 1;
    }
}
