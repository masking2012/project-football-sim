using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Shared.Features.EntityFrameworkCore;

namespace ProjectFootballSim.Calendar.Infrastructure.Repositories;

internal sealed class CreateGameCalendarRepository(CalendarDbContext dbContext) : ICreateGameCalendarRepository
{
    public async Task CreateAndSaveAsync(Guid userId, Guid gameId, DateTime initialDate, CancellationToken cancellationToken)
    {
        var gameCalendar = new GameCalendar(userId, gameId, initialDate);
        dbContext.GameCalendars.Add(gameCalendar);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new GameCalendarExistsException("A game calendar for the specified user and game already exists.", ex);
        }
    }
}
