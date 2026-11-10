using Moq;
using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Application.Features.CompleteGameSeason;
using ProjectFootballSim.Calendar.Contracts.Features.CompleteGameSeason;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;

namespace ProjectFootballSim.Calendar.Application.Tests.Features.CompleteGameSeason;

internal sealed class CompleteGameSeasonCommandHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid SeasonId = Guid.NewGuid();
    private static readonly DateTime SeasonEndDate = new(2026, 7, 31);
    private static readonly CompleteGameSeasonCommand Command = new(UserId, GameId, SeasonId);

    private readonly Mock<IGameCalendarWriteRepository> _writeRepository = new();
    private readonly Mock<IGameCalendarRepository> _gameCalendarRepository = new();

    private CompleteGameSeasonCommandHandler CreateSut() => new(_writeRepository.Object, _gameCalendarRepository.Object);

    [Test]
    public async Task ShouldThrowWhenCalendarNotFound()
    {
        _writeRepository
            .Setup(value => value.GetCurrentCalendarAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GameCalendar?)null);

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldThrowWhenDayIsNotCompleted()
    {
        SetupCalendar(new GameCalendar(UserId, GameId, SeasonEndDate));

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldThrowWhenSeasonNotFound()
    {
        SetupCalendar(CreateCompletedCalendar(SeasonEndDate));
        _gameCalendarRepository
            .Setup(value => value.GetCurrentSeasonAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GameSeasonDto?)null);

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldThrowWhenSeasonDoesNotMatchRequestedSeason()
    {
        SetupCalendar(CreateCompletedCalendar(SeasonEndDate));
        SetupSeason(new GameSeasonDto(Guid.NewGuid(), SeasonEndDate.AddYears(-1).AddDays(1), SeasonEndDate));

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldThrowWhenCurrentDateIsNotSeasonEndDate()
    {
        SetupCalendar(CreateCompletedCalendar(SeasonEndDate.AddDays(-1)));
        SetupSeason(new GameSeasonDto(SeasonId, SeasonEndDate.AddYears(-1).AddDays(1), SeasonEndDate));

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldAdvanceCalendarOneDayPastSeasonEnd()
    {
        var calendar = CreateCompletedCalendar(SeasonEndDate);
        SetupCalendar(calendar);
        SetupSeason(new GameSeasonDto(SeasonId, SeasonEndDate.AddYears(-1).AddDays(1), SeasonEndDate));

        await CreateSut().HandleAsync(Command, CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(calendar.CurrentDate).IsEqualTo(SeasonEndDate.AddDays(1));
            await Assert.That(calendar.DayStatus).IsEqualTo(CalendarDayStatus.NotStarted);
        }

        _writeRepository.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static GameCalendar CreateCompletedCalendar(DateTime currentDate)
    {
        var calendar = new GameCalendar(UserId, GameId, currentDate);
        calendar.UpdateDayStatus(CalendarDayStatus.Completed);
        return calendar;
    }

    private void SetupCalendar(GameCalendar calendar) =>
        _writeRepository
            .Setup(value => value.GetCurrentCalendarAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(calendar);

    private void SetupSeason(GameSeasonDto season) =>
        _gameCalendarRepository
            .Setup(value => value.GetCurrentSeasonAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
}
