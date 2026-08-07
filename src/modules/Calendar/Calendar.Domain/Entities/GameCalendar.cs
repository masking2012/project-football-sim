namespace ProjectFootballSim.Calendar.Domain.Entities;

public sealed class GameCalendar
{
    public Guid GameId { get; }
    public DateTime CurrentDate { get; private set; }

    public GameCalendar(Guid gameId, DateTime currentDate)
    {
        GameId = gameId;
        CurrentDate = currentDate;
    }

    public void UpdateDate(DateTime newDate)
    {
        if (newDate <= CurrentDate)
            throw new InvalidOperationException("New date cannot be earlier than or equal to the current date.");

        CurrentDate = newDate;
    }
}
