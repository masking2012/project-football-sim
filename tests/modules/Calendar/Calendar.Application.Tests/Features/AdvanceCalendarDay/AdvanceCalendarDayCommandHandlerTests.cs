using Moq;
using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Application.Features.AdvanceCalendarDay;
using ProjectFootballSim.Calendar.Contracts.Features.AdvanceCalendarDay;
using ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;

namespace ProjectFootballSim.Calendar.Application.Tests.Features.AdvanceCalendarDay;

internal sealed class AdvanceCalendarDayCommandHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly DateTime CurrentDate = new(2026, 8, 1);
    private static readonly AdvanceCalendarDayCommand Command = new(UserId, GameId);

    private readonly Mock<IGameCalendarWriteRepository> _writeRepository = new();
    private readonly Mock<IGameCalendarRepository> _gameCalendarRepository = new();
    private readonly Mock<IAdvanceCalendarDayRepository> _advanceCalendarDayRepository = new();
    private readonly Mock<ISimulateCalendarDayCommandHandler> _simulateHandler = new();

    private AdvanceCalendarDayCommandHandler CreateSut() => new(
        _writeRepository.Object,
        _gameCalendarRepository.Object,
        _advanceCalendarDayRepository.Object,
        _simulateHandler.Object);

    [Test]
    public async Task ShouldThrowWhenCalendarNotFound()
    {
        SetupCalendar(null);

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldThrowWhenCurrentDayIsNotCompleted()
    {
        SetupCalendar(new GameCalendar(UserId, GameId, CurrentDate));

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldThrowWhenCurrentSeasonNotFound()
    {
        var calendar = CreateCompletedCalendar();
        SetupCalendar(calendar);
        _gameCalendarRepository
            .Setup(value => value.GetCurrentSeasonAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GameSeasonDto?)null);

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldThrowWhenCurrentDateIsOnOrBeyondSeasonEnd()
    {
        var calendar = CreateCompletedCalendar();
        SetupCalendar(calendar);
        SetupSeason(new GameSeasonDto(Guid.NewGuid(), CurrentDate.AddYears(-1), CurrentDate));

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
        _writeRepository.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task ShouldAdvanceDateWithoutSimulatingWhenMatchesExist()
    {
        var calendar = CreateCompletedCalendar();
        SetupCalendar(calendar);
        SetupSeason(new GameSeasonDto(Guid.NewGuid(), CurrentDate.AddMonths(-1), CurrentDate.AddMonths(10)));
        _advanceCalendarDayRepository
            .Setup(value => value.MatchesExistAsync(UserId, GameId, CurrentDate.AddDays(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await CreateSut().HandleAsync(Command, CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(calendar.CurrentDate).IsEqualTo(CurrentDate.AddDays(1));
            await Assert.That(calendar.DayStatus).IsEqualTo(CalendarDayStatus.NotStarted);
        }

        _simulateHandler.Verify(value => value.HandleAsync(It.IsAny<SimulateCalendarDayCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        _writeRepository.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ShouldSimulateAndCompleteNewDayWhenNoMatchesExist()
    {
        var calendar = CreateCompletedCalendar();
        SetupCalendar(calendar);
        SetupSeason(new GameSeasonDto(Guid.NewGuid(), CurrentDate.AddMonths(-1), CurrentDate.AddMonths(10)));
        _advanceCalendarDayRepository
            .Setup(value => value.MatchesExistAsync(UserId, GameId, CurrentDate.AddDays(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await CreateSut().HandleAsync(Command, CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(calendar.CurrentDate).IsEqualTo(CurrentDate.AddDays(1));
            await Assert.That(calendar.DayStatus).IsEqualTo(CalendarDayStatus.Completed);
        }

        _simulateHandler.Verify(value => value.HandleAsync(
            It.Is<SimulateCalendarDayCommand>(command => command.UserId == UserId && command.GameId == GameId),
            It.IsAny<CancellationToken>()), Times.Once);
        _writeRepository.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static GameCalendar CreateCompletedCalendar()
    {
        var calendar = new GameCalendar(UserId, GameId, CurrentDate);
        calendar.UpdateDayStatus(CalendarDayStatus.Completed);
        return calendar;
    }

    private void SetupCalendar(GameCalendar? calendar) =>
        _writeRepository
            .Setup(value => value.GetCurrentCalendarAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(calendar);

    private void SetupSeason(GameSeasonDto season) =>
        _gameCalendarRepository
            .Setup(value => value.GetCurrentSeasonAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(season);
}
