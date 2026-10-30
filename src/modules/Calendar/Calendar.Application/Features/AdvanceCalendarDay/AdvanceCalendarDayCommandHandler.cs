using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Contracts.Features.AdvanceCalendarDay;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.CheckGameLeagueMatchesByDate;

namespace ProjectFootballSim.Calendar.Application.Features.AdvanceCalendarDay;

internal sealed class AdvanceCalendarDayCommandHandler(
    IGameCalendarWriteRepository repository,
    IGameCalendarRepository gameCalendarRepository,
    ICheckGameLeagueMatchesByDateQueryHandler checkGameLeagueMatchesByDateQueryHandler)
    : IAdvanceCalendarDayCommandHandler
{
    public async Task HandleAsync(AdvanceCalendarDayCommand command, CancellationToken cancellationToken)
    {
        GameCalendar? gameCalendar = await repository
            .GetCurrentCalendarAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);
        if (gameCalendar is null)
            throw new InvalidOperationException($"Game calendar not found for user '{command.UserId}' and game '{command.GameId}'");

        ValidateCurrentDayStatus(gameCalendar);
        await ValidateSeasonBoundariesAsync(gameCalendar, command, cancellationToken).ConfigureAwait(false);

        DateTime nextDay = gameCalendar.CurrentDate.AddDays(1);
        gameCalendar.UpdateDate(nextDay);

        var queryResult = await checkGameLeagueMatchesByDateQueryHandler
            .HandleAsync(
                new CheckGameLeagueMatchesByDateQuery(
                    UserId: command.UserId,
                    GameId: command.GameId,
                    Date: nextDay),
                cancellationToken)
            .ConfigureAwait(false);

        if (queryResult.MatchesExists)
            gameCalendar.UpdateDayStatus(CalendarDayStatus.Completed);

        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task ValidateSeasonBoundariesAsync(GameCalendar gameCalendar, AdvanceCalendarDayCommand command, CancellationToken cancellationToken)
    {
        GameSeasonDto? season = await gameCalendarRepository
            .GetCurrentSeasonAsync(command.UserId, command.GameId, cancellationToken)
            .ConfigureAwait(false);

        if (season is null)
            throw new InvalidOperationException($"Current game season not found for user ID '{command.UserId}' and game ID '{command.GameId}'.");

        if (gameCalendar.CurrentDate >= season.EndDate)
            throw new InvalidOperationException($"Cannot advance calendar. Current date '{gameCalendar.CurrentDate}' is beyond the season end date '{season.EndDate}'");
    }

    private static void ValidateCurrentDayStatus(GameCalendar gameCalendar)
    {
        if (gameCalendar.DayStatus != CalendarDayStatus.Completed)
            throw new InvalidOperationException("Cannot advance calendar. Current day is not completed.");
    }
}
