using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectFootballSim.Calendar.Contracts;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Shared.Data.Entities.Seasons;
using ProjectFootballSim.Shared.Features.EntityFrameworkCore;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;

public sealed class CreateGameSeasonCommandHandler(
    ILogger<CreateGameSeasonCommandHandler> logger,
    CalendarDbContext dbContext,
    IEventPublisher eventPublisher)
{
    public async Task<CreateGameSeasonCommandResult> HandleAsync(
        CreateGameSeasonCommand command,
        CancellationToken cancellationToken)
    {
        GameSeason? lastSeason = await dbContext.GameSeasons
            .Where(s => s.UserId == command.UserId && s.GameId == command.GameId && s.IsCurrent)
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        GameSeason newSeason;

        if (lastSeason is null)
        {
            var seasonDefinition = SeasonsDataProvider.GetSeasonDefinition();
            DateTime startDate = new DateTime(seasonDefinition.FirstSeasonYear, seasonDefinition.StartSeasonMonth, seasonDefinition.StartSeasonDay);
            DateTime endDate = startDate.AddYears(1).AddDays(-1);

            newSeason = new GameSeason(
                userId: command.UserId,
                gameId: command.GameId,
                startDate: startDate,
                endDate: endDate,
                order: 1);
            dbContext.GameSeasons.Add(newSeason);
        }
        else
        {
            lastSeason.CloseSeason();

            DateTime startDate = lastSeason.EndDate.AddDays(1);
            DateTime endDate = startDate.AddYears(1).AddDays(-1);

            newSeason = new GameSeason(
                userId: command.UserId,
                gameId: command.GameId,
                startDate: startDate,
                endDate: endDate,
                order: lastSeason.Order + 1);
            dbContext.GameSeasons.Add(newSeason);
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            Log.GameSeasonCreated(logger, newSeason.Id, newSeason.UserId, newSeason.GameId);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            Log.GameSeasonExists(logger, newSeason.UserId, newSeason.GameId, newSeason.Order, ex);
            throw;
        }

        await eventPublisher.PublishAsync(new GameSeasonStartedEvent(
            UserId: newSeason.UserId,
            GameId: newSeason.GameId,
            SeasonId: newSeason.Id,
            StartDate: newSeason.StartDate,
            PreviousSeasonId: lastSeason?.Id), cancellationToken).ConfigureAwait(false);

        return new CreateGameSeasonCommandResult(newSeason.Id, newSeason.StartDate, newSeason.EndDate);
    }
}
