using System.Security.Cryptography;

namespace ProjectFootballSim.Common.Features;

public static class CryptoRandom
{
    public static double NextDouble()
    {
        Span<byte> bytes = stackalloc byte[8];
        RandomNumberGenerator.Fill(bytes);
        ulong value = BitConverter.ToUInt64(bytes);

        // Keep the top 53 bits (double precision mantissa)
        return (value >> 11) * (1.0 / (1UL << 53));
    }

    /// <summary>
    /// The classic algorithm by Donald Knuth is simple and works well for football because λ is usually below 20.
    /// </summary>
    /// <param name="lambda"></param>
    /// <returns></returns>
    public static int NextPoisson(double lambda)
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
