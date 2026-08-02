using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Common.Data.Entities.Seasons;
using ProjectFootballSim.Seasons.Domain.Entities;
using ProjectFootballSim.Seasons.Infrastructure.Database;

namespace ProjectFootballSim.Seasons.Application.Features.CreateNextPlayerSeason;

public sealed class CreateNextPlayerSeasonCommandHandler(SeasonsDbContext dbContext)
{
    public async Task HandleAsync(CreatePlayerSeasonCommand command, CancellationToken cancellationToken)
    {
        PlayerSeason? lastSeason = await dbContext.PlayerSeasons
            .Where(s => s.UserId == command.UserId && s.IsCurrent)
            .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (lastSeason is null)
        {
            var seasonDefinition = SeasonsDataProvider.GetSeasonDefinition();
            DateTime startDate = new DateTime(seasonDefinition.StartYear, seasonDefinition.StartMonth, seasonDefinition.StartDay);
            DateTime endDate = startDate.AddYears(1).AddDays(-1);

            var newPlayerSeason = new PlayerSeason(
                gameId: command.GameId,
                userId: command.UserId,
                startDate: startDate,
                endDate: endDate,
                order: 1);
            dbContext.PlayerSeasons.Add(newPlayerSeason);
        }
        else
        {
            if (command.CurrentDate != lastSeason.EndDate)
                throw new InvalidOperationException("Cannot create a new season before the current season ends.");

            lastSeason.CloseSeason();

            DateTime startDate = command.CurrentDate.AddDays(1);
            DateTime endDate = startDate.AddYears(1).AddDays(-1);

            var newPlayerSeason = new PlayerSeason(
                gameId: command.GameId,
                userId: command.UserId,
                startDate: startDate,
                endDate: endDate,
                order: lastSeason.Order + 1);
            dbContext.PlayerSeasons.Add(newPlayerSeason);
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
