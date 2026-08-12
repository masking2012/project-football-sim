using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

namespace ProjectFootballSim.Calendar.Application.Features.AdvanceCalendarDay;

public sealed class AdvanceCalendarDayCommandHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    CalendarDbContext dbContext,
    GetGameLeagueMatchesByDateQueryHandler getGameLeagueMatchesByDateQueryHandler)
{
    public async Task HandleAsync(AdvanceCalendarDayCommand command, CancellationToken cancellationToken)
    {
        GameCalendar gameCalendar = await gameCalendarRetriever
            .GetCurrentGameDateAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);

        DateTime currentDate = gameCalendar.CurrentDate;
        var matches = await getGameLeagueMatchesByDateQueryHandler
            .HandleAsync(
                new GetGameLeagueMatchesByDateQuery(
                    UserId: command.UserId,
                    GameId: command.GameId,
                    Date: currentDate),
                cancellationToken)
            .ConfigureAwait(false);

        if (matches.Count > 0 && gameCalendar.DayStatus != CalendarDayStatus.Completed)
            throw new InvalidOperationException("Cannot advance calendar. Current day is not completed.");

        gameCalendar.UpdateDate(currentDate.AddDays(1));

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
