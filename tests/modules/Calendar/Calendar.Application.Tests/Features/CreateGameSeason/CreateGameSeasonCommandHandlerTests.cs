using Microsoft.Extensions.Logging;
using Moq;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Shared.Data.Entities.Seasons;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application.Tests.Features.CreateGameSeason;

internal sealed class CreateGameSeasonCommandHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly CreateGameSeasonCommand Command = new(UserId, GameId);

    private readonly Mock<ILogger<CreateGameSeasonCommandHandler>> _logger = new();
    private readonly Mock<IGameCalendarRepository> _gameCalendarRepository = new();
    private readonly Mock<ICreateGameSeasonRepository> _createGameSeasonRepository = new();
    private readonly Mock<IEventPublisher> _eventPublisher = new();

    private CreateGameSeasonCommandHandler CreateSut() => new(
        _logger.Object,
        _gameCalendarRepository.Object,
        _createGameSeasonRepository.Object,
        _eventPublisher.Object);

    private static DateTime FirstSeasonStartDate
    {
        get
        {
            var definition = SeasonsDataProvider.GetSeasonDefinition();
            return new DateTime(definition.FirstSeasonYear, definition.StartSeasonMonth, definition.StartSeasonDay);
        }
    }

    [Test]
    public async Task ShouldCreateFirstSeasonWhenNoCurrentSeasonExists()
    {
        var startDate = FirstSeasonStartDate;
        SetupCurrentSeason(null);
        SetupCalendar(new GameCalendarDto(startDate, GameDayStatus.NotStarted));

        var result = await CreateSut().HandleAsync(Command, CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(result.StartDate).IsEqualTo(startDate);
            await Assert.That(result.EndDate).IsEqualTo(startDate.AddYears(1).AddDays(-1));
        }

        _createGameSeasonRepository.Verify(value => value.Add(It.Is<GameSeason>(season => season.Order == 1)), Times.Once);
        _createGameSeasonRepository.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisher.Verify(value => value.PublishAsync(
            It.Is<GameSeasonStartedEvent>(@event => @event.PreviousSeasonId == null && @event.StartDate == startDate),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ShouldThrowWhenCalendarDateDoesNotMatchFirstSeasonStart()
    {
        var startDate = FirstSeasonStartDate;
        SetupCurrentSeason(null);
        SetupCalendar(new GameCalendarDto(startDate.AddDays(1), GameDayStatus.NotStarted));

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
        _createGameSeasonRepository.Verify(value => value.Add(It.IsAny<GameSeason>()), Times.Never);
    }

    [Test]
    public async Task ShouldThrowWhenCalendarDayAlreadyStarted()
    {
        var startDate = FirstSeasonStartDate;
        SetupCurrentSeason(null);
        SetupCalendar(new GameCalendarDto(startDate, GameDayStatus.Completed));

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldThrowWhenCalendarNotFound()
    {
        SetupCurrentSeason(null);
        _gameCalendarRepository
            .Setup(value => value.GetCurrentCalendarAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GameCalendarDto?)null);

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldCloseCurrentSeasonAndCreateNextSeason()
    {
        var previousStart = FirstSeasonStartDate;
        var previousEnd = previousStart.AddYears(1).AddDays(-1);
        var currentSeason = new GameSeason(UserId, GameId, previousStart, previousEnd, order: 1);
        var nextStart = previousEnd.AddDays(1);
        SetupCurrentSeason(currentSeason);
        SetupCalendar(new GameCalendarDto(nextStart, GameDayStatus.NotStarted));

        var result = await CreateSut().HandleAsync(Command, CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(currentSeason.IsCurrent).IsFalse();
            await Assert.That(result.StartDate).IsEqualTo(nextStart);
        }

        _createGameSeasonRepository.Verify(value => value.Add(It.Is<GameSeason>(season => season.Order == 2)), Times.Once);
        _eventPublisher.Verify(value => value.PublishAsync(
            It.Is<GameSeasonStartedEvent>(@event => @event.PreviousSeasonId == currentSeason.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupCurrentSeason(GameSeason? season) =>
        _createGameSeasonRepository
            .Setup(value => value.GetCurrentSeasonAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);

    private void SetupCalendar(GameCalendarDto calendar) =>
        _gameCalendarRepository
            .Setup(value => value.GetCurrentCalendarAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(calendar);
}
