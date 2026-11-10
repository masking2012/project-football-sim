using Moq;
using ProjectFootballSim.Calendar.Application.Features.GetGameSeasons;
using ProjectFootballSim.Calendar.Contracts.Features.GetGameSeasons;

namespace ProjectFootballSim.Calendar.Application.Tests.Features.GetGameSeasons;

internal sealed class GetGameSeasonsQueryHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid GameId = Guid.NewGuid();

    private readonly Mock<IGetGameSeasonsRepository> _repository = new();

    private GetGameSeasonsQueryHandler CreateSut() => new(_repository.Object);

    [Test]
    public async Task ShouldThrowWhenNoSeasonsFound()
    {
        _repository
            .Setup(value => value.GetAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        Func<Task> act = () => CreateSut().HandleAsync(new GetGameSeasonsQuery(UserId, GameId), CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldMapSeasonsToContractDtos()
    {
        var seasonId = Guid.NewGuid();
        var startDate = new DateTime(2026, 8, 1);
        var endDate = new DateTime(2027, 7, 31);
        _repository
            .Setup(value => value.GetAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new GameSeasonDto(seasonId, startDate, endDate, 1, true)]);

        var result = await CreateSut().HandleAsync(new GetGameSeasonsQuery(UserId, GameId), CancellationToken.None).ConfigureAwait(false);

        var season = result.Single();
        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(1);
            await Assert.That(season.Id).IsEqualTo(seasonId);
            await Assert.That(season.StartDate).IsEqualTo(startDate);
            await Assert.That(season.EndDate).IsEqualTo(endDate);
            await Assert.That(season.Order).IsEqualTo(1);
            await Assert.That(season.IsCurrent).IsTrue();
        }
    }
}
