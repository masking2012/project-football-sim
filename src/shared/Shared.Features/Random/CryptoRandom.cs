using System.Security.Cryptography;

namespace ProjectFootballSim.Shared.Features.Random;

internal sealed class CryptoRandom : IRandomNumberGenerator
{
    /// <summary>
    /// Returns a cryptographically secure random value in the range [0.0, 1.0), including 0.0 but excluding 1.0.
    /// </summary>
    public double NextDouble()
    {
        Span<byte> bytes = stackalloc byte[8];
        RandomNumberGenerator.Fill(bytes);
        ulong value = BitConverter.ToUInt64(bytes);

        // Keep the top 53 bits (double precision mantissa)
        return (value >> 11) * (1.0 / (1UL << 53));
    }

    /// <summary>
    /// Samples a Poisson-distributed event count with mean <paramref name="lambda"/> using Knuth's multiplicative algorithm.
    /// Returns 0 when <paramref name="lambda"/> is non-positive; intended for the small rates typical of football simulations.
    /// </summary>
    /// <param name="lambda"></param>
    /// <returns></returns>
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
