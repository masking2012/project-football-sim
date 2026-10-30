using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Infrastructure.Repositories;

internal sealed class GameCalendarRepository(CalendarDbContext dbContext) : IGameCalendarRepository
{
    public async Task<GameCalendarDto?> GetCurrentCalendarAsync(Guid userId, Guid gameId, CancellationToken cancellationToken)
    {
        GameCalendar? gameCalendar = await dbContext.GameCalendars
           .AsNoTracking()
           .SingleOrDefaultAsync(x => x.UserId == userId && x.GameId == gameId, cancellationToken)
           .ConfigureAwait(false);

        if (gameCalendar is null)
            return null;

        return new GameCalendarDto(CurrentDate: gameCalendar.CurrentDate, DayStatus: (GameDayStatus)gameCalendar.DayStatus);
    }

    public async Task<GameSeasonDto?> GetCurrentSeasonAsync(Guid userId, Guid gameId, CancellationToken cancellationToken)
    {
        GameSeason? gameSeason = await dbContext.GameSeasons
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId && x.GameId == gameId && x.IsCurrent, cancellationToken)
            .ConfigureAwait(false);

        if (gameSeason is null)
            return null;

        return new GameSeasonDto(Id: gameSeason.Id, StartDate: gameSeason.StartDate, EndDate: gameSeason.EndDate);
    }
}
