using Microsoft.Extensions.Logging;
using Moq;
using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameCalendar;
using ProjectFootballSim.Shared.Data.Entities.Seasons;

namespace ProjectFootballSim.Calendar.Application.Tests.Features.CreateGameCalendar;

internal sealed class CreateGameCalendarCommandHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid GameId = Guid.NewGuid();

    [Test]
    public async Task ShouldCreateCalendarAtSeasonStartDate()
    {
        var seasonDefinition = SeasonsDataProvider.GetSeasonDefinition();
        var expectedDate = new DateTime(seasonDefinition.FirstSeasonYear, seasonDefinition.StartSeasonMonth, seasonDefinition.StartSeasonDay);
        var logger = new Mock<ILogger<CreateGameCalendarCommandHandler>>();
        var repository = new Mock<ICreateGameCalendarRepository>();

        var handler = new CreateGameCalendarCommandHandler(logger.Object, repository.Object);
        await handler.HandleAsync(new CreateGameCalendarCommand(UserId, GameId), CancellationToken.None).ConfigureAwait(false);

        repository.Verify(value => value.CreateAndSaveAsync(UserId, GameId, expectedDate, It.IsAny<CancellationToken>()), Times.Once);
    }
}
