namespace ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;

public sealed class GameCalendarExistsException : Exception
{
    public GameCalendarExistsException()
    {
    }

    public GameCalendarExistsException(string message) : base(message)
    {
    }

    public GameCalendarExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
