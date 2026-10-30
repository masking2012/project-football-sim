using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;

namespace ProjectFootballSim.Calendar.Application.Features.CompleteGameSeason;

public sealed class CompleteGameSeasonCommandHandler(
    IGameCalendarWriteRepository repository,
    IGameCalendarRepository gameCalendarRepository,
    ICreateGameSeasonCommandHandler createGameSeasonCommandHandler)
{
    public async Task<CompleteGameSeasonCommandResult> HandleAsync(CompleteGameSeasonCommand command, CancellationToken cancellationToken)
    {
        GameCalendar? gameCalendar = await repository.GetCurrentCalendarAsync(
            userId: command.UserId,
            gameId: command.GameId, cancellationToken).ConfigureAwait(false);
        if (gameCalendar is null)
            throw new InvalidOperationException($"Game calendar not found for user '{command.UserId}' and game '{command.GameId}'");

        await ValidateGameCalendarAsync(gameCalendar, command, cancellationToken)
            .ConfigureAwait(false);

        DateTime nextDay = gameCalendar.CurrentDate.AddDays(1);
        gameCalendar.UpdateDate(nextDay);
        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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

    private async Task ValidateGameCalendarAsync(
        GameCalendar gameCalendar,
        CompleteGameSeasonCommand command,
        CancellationToken cancellationToken)
    {
        if (gameCalendar.DayStatus != CalendarDayStatus.Completed)
            throw new InvalidOperationException("Current game calendar day isn't completed");

        GameSeasonDto? currentSeason = await gameCalendarRepository
            .GetCurrentSeasonAsync(userId: command.UserId, gameId: command.GameId, cancellationToken)
            .ConfigureAwait(false);
        if (currentSeason is null)
            throw new InvalidOperationException("Current game season not found");

        if (gameCalendar.CurrentDate < currentSeason.EndDate)
            throw new InvalidOperationException("Game calendar day haven't reach the end of the season");
    }
}
