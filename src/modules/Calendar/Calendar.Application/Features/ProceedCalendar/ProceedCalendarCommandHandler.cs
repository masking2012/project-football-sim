using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

namespace ProjectFootballSim.Calendar.Application.Features.ProceedCalendar;

public sealed class ProceedCalendarCommandHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    CalendarDbContext dbContext,
    GetGameLeagueMatchesByDateQueryHandler getGameLeagueMatchesByDateQueryHandler)
{
    public async Task HandleAsync(ProceedCalendarCommand command, CancellationToken cancellationToken)
    {
        GameCalendar gameCalendar = await gameCalendarRetriever
            .GetCurrentGameDateAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);

        var getGameLeagueMatchesByDateQuery = new GetGameLeagueMatchesByDateQuery(GameId: command.GameId, Date: gameCalendar.CurrentDate);
        var matches = await getGameLeagueMatchesByDateQueryHandler
            .HandleAsync(getGameLeagueMatchesByDateQuery, cancellationToken)
            .ConfigureAwait(false);

        if (gameCalendar.State != Domain.Enums.DayState.Completed && matches is not null && matches.Any())
            throw new InvalidOperationException("Cannot proceed calendar. Current day is not completed.");

        gameCalendar.UpdateDate(gameCalendar.CurrentDate.AddDays(1));
        gameCalendar.UpdateState(Domain.Enums.DayState.NotStarted);

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
