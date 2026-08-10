using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Application.Features.AdvanceCalendarDay;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

namespace ProjectFootballSim.Calendar.Application.Features.ProceedCalendar;

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

        var getGameLeagueMatchesByDateQuery = new GetGameLeagueMatchesByDateQuery(UserId: command.UserId, GameId: command.GameId, Date: gameCalendar.CurrentDate);
        var matches = await getGameLeagueMatchesByDateQueryHandler
            .HandleAsync(getGameLeagueMatchesByDateQuery, cancellationToken)
            .ConfigureAwait(false);

        if (gameCalendar.DayStatus != Domain.Enums.CalendarDayStatus.Completed && matches.Any())
            throw new InvalidOperationException("Cannot advance calendar. Current day is not completed.");

        gameCalendar.UpdateDate(gameCalendar.CurrentDate.AddDays(1));

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
