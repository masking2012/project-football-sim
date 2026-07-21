using System.Security.Cryptography;

namespace Common.Features;

public static class CryptoRandom
{
    public static double NextDouble()
    {
        ulong value = (ulong)RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
        value = (value << 32) | (uint)RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);

        // Keep the top 53 bits (double precision mantissa)
        return (value >> 11) * (1.0 / (1UL << 53));
    }
}
