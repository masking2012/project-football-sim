using Microsoft.Extensions.Logging;
using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Shared.Data.Entities.Seasons;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;

public sealed class CreateGameSeasonCommandHandler(
    ILogger<CreateGameSeasonCommandHandler> logger,
    IGetGameCalendarRepository getGameCalendarRepository,
    ICreateGameSeasonRepository createGameSeasonRepository,
    IEventPublisher eventPublisher) : ICreateGameSeasonCommandHandler
{
    public async Task<CreateGameSeasonCommandResult> HandleAsync(
        CreateGameSeasonCommand command,
        CancellationToken cancellationToken)
    {
        GameSeason? currentSeason = await createGameSeasonRepository.GetCurrentSeasonAsync(command.UserId, command.GameId, cancellationToken).ConfigureAwait(false);
        GameSeason newSeason;

        if (currentSeason is null)
        {
            var seasonDefinition = SeasonsDataProvider.GetSeasonDefinition();
            DateTime startDate = new DateTime(seasonDefinition.FirstSeasonYear, seasonDefinition.StartSeasonMonth, seasonDefinition.StartSeasonDay);
            DateTime endDate = startDate.AddYears(1).AddDays(-1);

            await ValidateStartDateAsync(command, startDate, cancellationToken).ConfigureAwait(false);

            newSeason = new GameSeason(
                userId: command.UserId,
                gameId: command.GameId,
                startDate: startDate,
                endDate: endDate,
                order: 1);
            createGameSeasonRepository.Add(newSeason);
        }
        else
        {
            currentSeason.CloseSeason();

            DateTime startDate = currentSeason.EndDate.AddDays(1);
            DateTime endDate = startDate.AddYears(1).AddDays(-1);

            await ValidateStartDateAsync(command, startDate, cancellationToken).ConfigureAwait(false);

            newSeason = new GameSeason(
                userId: command.UserId,
                gameId: command.GameId,
                startDate: startDate,
                endDate: endDate,
                order: currentSeason.Order + 1);
            createGameSeasonRepository.Add(newSeason);
        }

        try
        {
            await createGameSeasonRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            CreateGameSeasonLogs.Created(logger, newSeason.Id, newSeason.UserId, newSeason.GameId);
        }
        catch (GameSeasonExistsException ex)
        {
            CreateGameSeasonLogs.Exists(logger, newSeason.UserId, newSeason.GameId, newSeason.Order, ex.InnerException);
            throw;
        }

        await eventPublisher.PublishAsync(new GameSeasonStartedEvent(
            UserId: newSeason.UserId,
            GameId: newSeason.GameId,
            SeasonId: newSeason.Id,
            StartDate: newSeason.StartDate,
            PreviousSeasonId: currentSeason?.Id), cancellationToken).ConfigureAwait(false);

        return new CreateGameSeasonCommandResult(newSeason.Id, newSeason.StartDate, newSeason.EndDate);
    }

    private async Task ValidateStartDateAsync(CreateGameSeasonCommand command, DateTime startDate, CancellationToken cancellationToken)
    {
        GameCalendarDto? gameCalendar = await getGameCalendarRepository
            .GetAsync(userId: command.UserId, gameId: command.GameId, cancellationToken)
            .ConfigureAwait(false);

        if (gameCalendar is null)
            throw new InvalidOperationException(
                $"Game calendar not found for user ID '{command.UserId}' and game ID '{command.GameId}'.");

        if (gameCalendar.CurrentDate != startDate)
            throw new InvalidOperationException(
                $"Cannot start the season because the current game date '{gameCalendar.CurrentDate}' does not match the season start date '{startDate}'.");

        if (gameCalendar.DayStatus != GameDayStatus.NotStarted)
            throw new InvalidOperationException("Cannot start the season because the current game day has already started");
    }
}
