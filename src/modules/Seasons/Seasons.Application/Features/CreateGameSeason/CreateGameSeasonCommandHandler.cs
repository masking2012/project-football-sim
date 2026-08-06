using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Common.Data.Entities.Seasons;
using ProjectFootballSim.Seasons.Domain.Entities;
using ProjectFootballSim.Seasons.Infrastructure.Database;

namespace ProjectFootballSim.Seasons.Application.Features.CreatePlayerSeason;

public sealed class CreateGameSeasonCommandHandler(SeasonsDbContext dbContext)
{
    public async Task<CreateGameSeasonCommandResult> HandleAsync(
        CreateGameSeasonCommand command,
        CancellationToken cancellationToken)
    {
        PlayerSeason? lastSeason = await dbContext.PlayerSeasons
            .Where(s => s.UserId == command.UserId && s.GameId == command.GameId && s.IsCurrent)
            .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        PlayerSeason newSeason;

        if (lastSeason is null)
        {
            var seasonDefinition = SeasonsDataProvider.GetSeasonDefinition();
            DateTime startDate = new DateTime(seasonDefinition.StartYear, seasonDefinition.StartMonth, seasonDefinition.StartDay);
            DateTime endDate = startDate.AddYears(1).AddDays(-1);

            newSeason = new PlayerSeason(
                gameId: command.GameId,
                userId: command.UserId,
                startDate: startDate,
                endDate: endDate,
                order: 1);
            dbContext.PlayerSeasons.Add(newSeason);
        }
        else
        {
            if (command.CurrentGameDate != lastSeason.EndDate)
                throw new InvalidOperationException("Cannot create a new season before the current season ends.");

            lastSeason.CloseSeason();

            DateTime startDate = command.CurrentGameDate.AddDays(1);
            DateTime endDate = startDate.AddYears(1).AddDays(-1);

            newSeason = new PlayerSeason(
                gameId: command.GameId,
                userId: command.UserId,
                startDate: startDate,
                endDate: endDate,
                order: lastSeason.Order + 1);
            dbContext.PlayerSeasons.Add(newSeason);
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new CreateGameSeasonCommandResult(newSeason.Id);
    }
}
