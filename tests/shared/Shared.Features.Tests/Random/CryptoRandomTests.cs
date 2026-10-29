using ProjectFootballSim.Shared.Features.Random;

namespace ProjectFootballSim.Shared.Features.Tests.Random;

internal sealed class CryptoRandomTests
{
    private readonly CryptoRandom _sut = new();

    [Test]
    public async Task ShouldGenerateValidValue()
    {
        foreach (var _ in Enumerable.Range(0, 1000))
        {
            var value = _sut.NextDouble();
            await Assert.That(value).IsGreaterThanOrEqualTo(0).And.IsLessThan(1);
        }
    }

    [Test]
    public async Task ShouldReturnZeroForNonPositiveLambda()
    {
        using (Assert.Multiple())
        {
            await Assert.That(_sut.NextPoisson(0)).IsEqualTo(0);
            await Assert.That(_sut.NextPoisson(-1)).IsEqualTo(0);
        }
    }

    [Test]
    public async Task ShouldGenerateNonNegativeValuesForPositiveLambda()
    {
        foreach (var _ in Enumerable.Range(0, 1000))
        {
            await Assert.That(_sut.NextPoisson(4)).IsGreaterThanOrEqualTo(0);
        }
    }

    [Test]
    public async Task ShouldGenerateValuesWithMeanNearLambda()
    {
        const int sampleCount = 10_000;
        const double lambda = 4;
        var total = 0;

        foreach (var _ in Enumerable.Range(0, sampleCount))
        {
            total += _sut.NextPoisson(lambda);
        }

        var mean = (double)total / sampleCount;

        await Assert.That(mean).IsBetween(3.8, 4.2);
    }
}
