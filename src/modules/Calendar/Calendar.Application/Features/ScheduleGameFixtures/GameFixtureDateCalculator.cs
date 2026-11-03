namespace ProjectFootballSim.Calendar.Application.Features.ScheduleGameFixtures;

internal static class GameFixtureDateCalculator
{
    public static IReadOnlyList<DateTime> CalculateDates(
        DateTime baseDate,
        IReadOnlySet<DayOfWeek> availableDaysOfWeek,
        int fixtureCount)
    {
        if (baseDate.DayOfWeek != DayOfWeek.Sunday)
            //TODO: provide logic for the rest of base dates
            throw new ArgumentException("Base date must be a Sunday.", nameof(baseDate));

        IReadOnlyList<int> dayOffsets = availableDaysOfWeek
            .Select(GetOffsetFromSunday)
            .Order()
            .ToList();

        List<DateTime> dates = [];
        DateTime weekBaseDate = baseDate.Date;
        while (dates.Count < fixtureCount)
        {
            dates.AddRange(dayOffsets
                .Select(offset => weekBaseDate.AddDays(offset))
                .Take(fixtureCount - dates.Count));
            weekBaseDate = weekBaseDate.AddDays(7);
        }

        return dates;
    }

    private static int GetOffsetFromSunday(DayOfWeek dayOfWeek)
    {
        int offset = (int)dayOfWeek;
        return offset > 3 ? offset - 7 : offset;
    }
}
