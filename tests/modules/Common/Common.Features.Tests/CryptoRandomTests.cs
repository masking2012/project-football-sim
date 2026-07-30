namespace ProjectFootballSim.Common.Features.Tests;

internal sealed class CryptoRandomTests
{
    [Test]
    public async Task ShouldGenerateValidValueAsync()
    {
        foreach (var _ in Enumerable.Range(0, 1000))
        {
            await Assert.That(CryptoRandom.NextDouble()).IsBetween(0, 1);
        }
    }
}
