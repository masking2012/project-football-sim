namespace ProjectFootballSim.Calendar.Application.Common.Models;

public sealed record GameCalendarDto(DateTime CurrentDate, GameDayStatus DayStatus);

public enum GameDayStatus
{
    NotStarted = 0,
    InProgress = 1, //TODO: not using yet. once we have full async simulation of a day, we can use this to indicate that the day is in progress and not yet completed
    Completed = 2
}
