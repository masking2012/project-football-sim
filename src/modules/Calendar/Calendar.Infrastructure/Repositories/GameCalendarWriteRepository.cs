using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Infrastructure.Repositories;

internal sealed class GameCalendarWriteRepository(CalendarDbContext dbContext) : IGameCalendarWriteRepository
{
    public async Task<GameCalendar?> GetCurrentCalendarAsync(Guid userId, Guid gameId, CancellationToken cancellationToken)
    {
        var gameCalendar = await dbContext.GameCalendars
            .SingleOrDefaultAsync(c => c.UserId == userId && c.GameId == gameId, cancellationToken)
            .ConfigureAwait(false);
        return gameCalendar;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
