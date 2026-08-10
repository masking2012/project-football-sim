using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Application.Common.Services;

public interface IGameCalendarRetriever
{
    Task<GameCalendar> GetCurrentGameDateAsync(Guid UserId, Guid GameId, CancellationToken cancellationToken); 
}

internal sealed class GameCalendarRetriever(CalendarDbContext dbContext) : IGameCalendarRetriever
{
    public async Task<GameCalendar> GetCurrentGameDateAsync(Guid UserId, Guid GameId, CancellationToken cancellationToken)
    {
        GameCalendar? gameCalendar = await dbContext.GameCalendars
            .SingleOrDefaultAsync(x => x.UserId == UserId && x.GameId == GameId, cancellationToken)
            .ConfigureAwait(false);

        if (gameCalendar is null)
            throw new InvalidOperationException($"Game calendar not found for UserId: {UserId}, GameId: {GameId}");

        return gameCalendar;
    }
}
