using ProjectFootballSim.Calendar.Domain.Enums;

namespace ProjectFootballSim.Calendar.Domain.Entities;

public sealed class GameCalendar
{
    public Guid UserId { get; }
    public Guid GameId { get; }
    public DateTime CurrentDate { get; private set; }
    public CalendarDayStatus DayStatus { get; private set; } 

    public GameCalendar(Guid userId, Guid gameId, DateTime currentDate)
    {
        UserId = userId;
        GameId = gameId;
        CurrentDate = currentDate;
        DayStatus = CalendarDayStatus.NotStarted;
    }

    public void UpdateDate(DateTime newDate)
    {
        if (newDate <= CurrentDate)
            throw new InvalidOperationException("New date cannot be earlier than or equal to the current date.");

        DayStatus = CalendarDayStatus.NotStarted;
        CurrentDate = newDate;
    }

    public void UpdateDayStatus(CalendarDayStatus newStatus)
    {
        if (newStatus == CalendarDayStatus.NotStarted && DayStatus == CalendarDayStatus.InProgress)
            throw new InvalidOperationException("Cannot revert to NotStarted from InProgress.");

        DayStatus = newStatus;
    }
}
