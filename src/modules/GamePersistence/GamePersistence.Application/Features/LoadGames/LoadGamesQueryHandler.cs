using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.GamePersistence.Infrastructure.Database;

namespace ProjectFootballSim.GamePersistence.Application.Features.LoadGames;

public sealed class LoadGamesQueryHandler(GamePersistenceDbContext dbContext)
{
    public async Task<IReadOnlyList<GameSaveDto>> HandleAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.GameSaves
            .AsNoTracking()
            .Where(gameSave => gameSave.UserId == userId)
            .Select(gameSave => new GameSaveDto(gameSave.GameId, gameSave.SlotId, gameSave.Name, gameSave.CreatedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
