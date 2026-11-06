using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Infrastructure.Repositories;

internal sealed class GetCalendarDayFixturesRepository(CalendarDbContext dbContext) : IGetCalendarDayFixturesRepository
{
    public async Task<IReadOnlyList<GameFixture>> GetFixturesAsync(
        Guid userId,
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        return await dbContext.GameFixtures
            .AsNoTracking()
            .Where(fixture => fixture.UserId == userId
                && fixture.GameId == gameId
                && fixture.ScheduledAt >= startDate
                && fixture.ScheduledAt < endDate)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
