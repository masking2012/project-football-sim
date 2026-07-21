using System.Security.Cryptography;

namespace Common.Features;

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
}
