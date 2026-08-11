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
                gameId: command.GameId,
                userId: command.UserId,
                startDate: startDate,
                endDate: endDate,
                order: lastSeason.Order + 1);
            dbContext.GameSeasons.Add(newSeason);
        }

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new CreateGameSeasonCommandResult(newSeason.Id, newSeason.StartDate, newSeason.EndDate);
    }
}
