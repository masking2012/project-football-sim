using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

namespace ProjectFootballSim.Calendar.Application.Features.AdvanceCalendarDay;

public sealed class AdvanceCalendarDayCommandHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    ISeasonsRetriever seasonsRetriever,
    CalendarDbContext dbContext,
    GetGameLeagueMatchesByDateQueryHandler getGameLeagueMatchesByDateQueryHandler)
{
    public async Task HandleAsync(AdvanceCalendarDayCommand command, CancellationToken cancellationToken)
    {
        GameCalendar gameCalendar = await gameCalendarRetriever
            .GetCurrentGameDateAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);

        ValidateCurrentDayStatus(gameCalendar);
        await ValidateSeasonBoundariesAsync(gameCalendar, command, cancellationToken).ConfigureAwait(false);

        DateTime nextDay = gameCalendar.CurrentDate.AddDays(1);
        gameCalendar.UpdateDate(nextDay);

        var nextDayMatches = await getGameLeagueMatchesByDateQueryHandler
            .HandleAsync(
                new GetGameLeagueMatchesByDateQuery(
                    UserId: command.UserId,
                    GameId: command.GameId,
                    Date: nextDay),
                cancellationToken)
            .ConfigureAwait(false);

        if (nextDayMatches.Count <= 0)
            gameCalendar.UpdateDayStatus(CalendarDayStatus.Completed);

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task ValidateSeasonBoundariesAsync(GameCalendar gameCalendar, AdvanceCalendarDayCommand command, CancellationToken cancellationToken)
    {
        GameSeason season = await seasonsRetriever
            .GetCurrentGameSeasonAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);

        if (gameCalendar.CurrentDate >= season.EndDate)
            throw new InvalidOperationException("Cannot advance calendar. Current date is beyond the season end date.");
    }

    private static void ValidateCurrentDayStatus(GameCalendar gameCalendar)
    {
        if (gameCalendar.DayStatus != CalendarDayStatus.Completed)
            throw new InvalidOperationException("Cannot advance calendar. Current day is not completed.");
    }
}
