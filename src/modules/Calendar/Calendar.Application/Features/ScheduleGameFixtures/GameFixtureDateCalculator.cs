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

        if (availableDaysOfWeek.Count == 0)
            throw new ArgumentException("At least one available day of week is required.", nameof(availableDaysOfWeek));

        List<int> dayOffsets = availableDaysOfWeek
            .Select(GetOffsetFromSunday)
            .Order()
            .ToList();

        int fixturesPerDate = fixtureCount / dayOffsets.Count;
        int remainder = fixtureCount % dayOffsets.Count;

        return dayOffsets
            .Select((offset, index) => (Date: baseDate.Date.AddDays(offset), Count: fixturesPerDate + (index < remainder ? 1 : 0)))
            .SelectMany(item => Enumerable.Repeat(item.Date, item.Count))
            .ToList();
    }

    private static int GetOffsetFromSunday(DayOfWeek dayOfWeek)
    {
        int offset = (int)dayOfWeek;
        return offset > 3 ? offset - 7 : offset;
    }
}
