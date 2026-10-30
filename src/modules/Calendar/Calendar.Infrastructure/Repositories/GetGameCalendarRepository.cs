using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Infrastructure.Repositories;

internal sealed class GetGameCalendarRepository(CalendarDbContext dbContext) : IGetGameCalendarRepository
{
    public async Task<GameCalendarDto?> GetAsync(Guid userId, Guid gameId, CancellationToken cancellationToken)
    {
        GameCalendar? gameCalendar = await dbContext.GameCalendars
           .SingleOrDefaultAsync(x => x.UserId == userId && x.GameId == gameId, cancellationToken)
           .ConfigureAwait(false);

        if (gameCalendar is null)
            return null;

        return new GameCalendarDto(CurrentDate: gameCalendar.CurrentDate, DayStatus: (GameDayStatus)gameCalendar.DayStatus);
    }
}
