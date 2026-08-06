namespace ProjectFootballSim.Common.Features;

public static class DateTimeUtils
{
    public static DateTime GetNextDayOfWeek(DateTime initialDate, DayOfWeek dayOfWeek)
    {
        int daysUntilTargetDay = ((int)dayOfWeek - (int)initialDate.DayOfWeek + 7) % 7;
        return initialDate.AddDays(daysUntilTargetDay);
    }
}
