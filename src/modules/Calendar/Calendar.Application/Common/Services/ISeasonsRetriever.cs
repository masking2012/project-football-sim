using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Application.Common.Services;

public interface ISeasonsRetriever
{
    Task<GameSeason> GetCurrentGameSeasonAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);
}

internal sealed class SeasonsRetriever(
    CalendarDbContext dbContext) : ISeasonsRetriever
{
    public async Task<GameSeason> GetCurrentGameSeasonAsync(Guid userId, Guid gameId, CancellationToken cancellationToken)
    {
        var gameSeason = await dbContext.GameSeasons
            .SingleOrDefaultAsync(gs => gs.UserId == userId && gs.GameId == gameId && gs.IsCurrent, cancellationToken)
            .ConfigureAwait(false);
        if (gameSeason is null)
            throw new InvalidOperationException("No active game season found for the specified user and game.");
        return gameSeason;
    }
}
