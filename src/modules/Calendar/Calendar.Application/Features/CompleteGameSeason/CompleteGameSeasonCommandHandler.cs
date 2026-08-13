using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Common.Data.Entities.Seasons;

namespace ProjectFootballSim.Calendar.Application.Features.CompleteGameSeason;

public sealed class CompleteGameSeasonCommandHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    CreateGameSeasonCommandHandler createGameSeasonCommandHandler)
{
    public async Task<CompleteGameSeasonCommandResult> HandleAsync(CompleteGameSeasonCommand command, CancellationToken cancellationToken)
    {
        var gameCalendar = await gameCalendarRetriever.GetCurrentGameDateAsync(
            UserId: command.UserId,
            GameId: command.GameId, cancellationToken).ConfigureAwait(false);
        ValidateGameCalendar(gameCalendar);

        var createGameSeasonCommandResult = await createGameSeasonCommandHandler.HandleAsync(new CreateGameSeasonCommand(
            UserId: command.UserId,
            GameId: command.GameId), cancellationToken).ConfigureAwait(false);

        return new CompleteGameSeasonCommandResult
        (
            Id: createGameSeasonCommandResult.Id,
            StartDate: createGameSeasonCommandResult.StartDate,
            EndDate: createGameSeasonCommandResult.EndDate
        );
    }

    private static void ValidateGameCalendar(GameCalendar gameCalendar)
    {
        if (gameCalendar.DayStatus != CalendarDayStatus.Completed)
            throw new InvalidOperationException("Current game calendar day isn't completed");

        var seasonDefinition = SeasonsDataProvider.GetSeasonDefinition();

        if (gameCalendar.CurrentDate.Month != seasonDefinition.EndSeasonMonth
            || gameCalendar.CurrentDate.Day != seasonDefinition.EndSeasonDay)
            throw new InvalidOperationException("Game calendar day haven't reach the end of the season");
    }
}
