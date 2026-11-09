using Moq;
using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Features.SimulateCalendarDay;
using ProjectFootballSim.Calendar.Contracts.Features.SimulateCalendarDay;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application.Tests.Features.SimulateCalendarDay;

internal sealed class SimulateCalendarDayCommandHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly DateTime CurrentDate = new(2026, 8, 1);
    private static readonly SimulateCalendarDayCommand Command = new(UserId, GameId);

    private readonly Mock<IGameCalendarWriteRepository> _repository = new();
    private readonly Mock<IEventPublisher> _eventPublisher = new();

    private SimulateCalendarDayCommandHandler CreateSut() => new(_repository.Object, _eventPublisher.Object);

    [Test]
    public async Task ShouldThrowWhenCalendarNotFound()
    {
        _repository
            .Setup(value => value.GetCurrentCalendarAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GameCalendar?)null);

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldThrowWhenDayAlreadyCompleted()
    {
        var calendar = new GameCalendar(UserId, GameId, CurrentDate);
        calendar.UpdateDayStatus(CalendarDayStatus.Completed);
        SetupCalendar(calendar);

        Func<Task> act = () => CreateSut().HandleAsync(Command, CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
        _eventPublisher.Verify(value => value.PublishAsync(It.IsAny<SimulateCalendarDayEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task ShouldPublishSimulationEventAndCompleteDay()
    {
        var calendar = new GameCalendar(UserId, GameId, CurrentDate);
        SetupCalendar(calendar);

        await CreateSut().HandleAsync(Command, CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(calendar.DayStatus).IsEqualTo(CalendarDayStatus.Completed);
            await Assert.That(calendar.CurrentDate).IsEqualTo(CurrentDate);
        }

        _eventPublisher.Verify(value => value.PublishAsync(
            It.Is<SimulateCalendarDayEvent>(@event => @event.UserId == UserId && @event.GameId == GameId && @event.Date == CurrentDate),
            It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupCalendar(GameCalendar calendar) =>
        _repository
            .Setup(value => value.GetCurrentCalendarAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(calendar);
}
