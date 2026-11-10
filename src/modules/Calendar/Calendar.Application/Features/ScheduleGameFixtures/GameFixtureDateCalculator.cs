namespace ProjectFootballSim.Calendar.Application.Features.ScheduleGameFixtures;

internal static class GameFixtureDateCalculator
{
    private static readonly DayOfWeek[] DayPriority =
    [
        DayOfWeek.Sunday,
        DayOfWeek.Saturday,
        DayOfWeek.Friday,
        DayOfWeek.Thursday,
        DayOfWeek.Wednesday,
        DayOfWeek.Tuesday,
        DayOfWeek.Monday
    ];

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

        List<DayOfWeek> prioritizedDays = DayPriority
            .Where(availableDaysOfWeek.Contains)
            .ToList();

        int fixturesPerDate = fixtureCount / prioritizedDays.Count;
        int remainder = fixtureCount % prioritizedDays.Count;

        return prioritizedDays
            .Select((dayOfWeek, index) => (Offset: GetOffsetFromSunday(dayOfWeek), Count: fixturesPerDate + (index < remainder ? 1 : 0)))
            .OrderBy(item => item.Offset)
            .SelectMany(item => Enumerable.Repeat(baseDate.Date.AddDays(item.Offset), item.Count))
            .ToList();
    }

    private static int GetOffsetFromSunday(DayOfWeek dayOfWeek)
    {
        int offset = (int)dayOfWeek;
        return offset > 3 ? offset - 7 : offset;
    }
}
