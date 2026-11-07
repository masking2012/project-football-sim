using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Application.Features.AdvanceCalendarDay;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Infrastructure.Repositories;

internal sealed class AdvanceCalendarDayRepository(CalendarDbContext dbContext) : IAdvanceCalendarDayRepository
{
    public async Task<bool> MatchesExistAsync(Guid userId, Guid gameId, DateTime calendarDate, CancellationToken cancellationToken)
    {
        DateTime startDate = calendarDate.Date;
        DateTime endDate = startDate.AddDays(1);

        return await dbContext.GameFixtures
            .AsNoTracking()
            .AnyAsync(fixture => fixture.UserId == userId
                && fixture.GameId == gameId
                && fixture.ScheduledAt >= startDate
                && fixture.ScheduledAt < endDate,
                cancellationToken)
            .ConfigureAwait(false);
    }
}
