using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Common.Data.Entities.Seasons;
using ProjectFootballSim.Common.Features.EntityFrameworkCore;

namespace ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;

public sealed class CreateGameCalendarCommandHandler(
    ILogger<CreateGameCalendarCommandHandler> logger,
    CalendarDbContext dbContext)
{
    public async Task HandleAsync(
        CreateGameCalendarCommand command,
        CancellationToken cancellationToken)
    {
        var seasonDefinition = SeasonsDataProvider.GetSeasonDefinition();
        var initialDate = new DateTime(seasonDefinition.FirstSeasonYear, seasonDefinition.StartSeasonMonth, seasonDefinition.StartSeasonDay);

        try
        {
            var gameCalendar = new GameCalendar(command.UserId, command.GameId, initialDate);
            dbContext.GameCalendars.Add(gameCalendar);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            Log.GameCalendarCreated(logger, command.UserId, command.GameId);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            Log.GameCalendarExists(logger, command.UserId, command.GameId, ex);
            throw;
        }
    }
}
