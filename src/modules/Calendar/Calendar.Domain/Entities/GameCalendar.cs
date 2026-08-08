using ProjectFootballSim.Calendar.Domain.Enums;

namespace ProjectFootballSim.Calendar.Domain.Entities;

public sealed class GameCalendar
{
    public Guid UserId { get; }
    public Guid GameId { get; }
    public DateTime CurrentDate { get; private set; }
    public DayState State { get; private set; } 

    public GameCalendar(Guid userId, Guid gameId, DateTime currentDate)
    {
        UserId = userId;
        GameId = gameId;
        CurrentDate = currentDate;
        State = DayState.NotStarted;
    }

    public void UpdateDate(DateTime newDate)
    {
        if (newDate <= CurrentDate)
            throw new InvalidOperationException("New date cannot be earlier than or equal to the current date.");

        CurrentDate = newDate;
    }

    public void UpdateState(DayState newState)
    {
        if (newState == DayState.NotStarted && State == DayState.InProgress)
            throw new InvalidOperationException("Cannot revert to NotStarted from InProgress.");

        State = newState;
    }
}
