using System.Security.Cryptography;

namespace ProjectFootballSim.Common.Features;

internal static class ListExtensions
{
    public static List<T> Shuffle<T>(this List<T> list)
    {
        Random rng = new();

        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1); // Pick a random index from 0 to i
            (list[i], list[j]) = (list[j], list[i]); // Swap
        }

        return list;
    }
}
