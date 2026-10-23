using ProjectFootballSim.Shared.Features.Random;

namespace ProjectFootballSim.Shared.Features.Tests.Random;

internal sealed class CryptoRandomTests
{
    private readonly CryptoRandom _sut = new();

    [Test]
    public async Task ShouldGenerateValidValueAsync()
    {
        foreach (var _ in Enumerable.Range(0, 1000))
        {
            await Assert.That(_sut.NextDouble()).IsBetween(0, 1);
        }
    }
}
