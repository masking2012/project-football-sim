using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.GamePersistence.Domain.Entities;
using ProjectFootballSim.GamePersistence.Infrastructure.Database;

namespace ProjectFootballSim.GamePersistence.Application.Features.CreateNewGame;

public sealed class SaveGameCommandHandler(GamePersistenceDbContext dbContext)
{
    public async Task HandleAsync(SaveGameCommand command, CancellationToken cancellationToken)
    {
        GameSave? existingSave = await dbContext.GameSaves
            .FirstOrDefaultAsync(gs => gs.UserId == command.UserId && gs.SlotId == command.SlotId, cancellationToken)
            .ConfigureAwait(false);
        if (existingSave is not null)
            dbContext.GameSaves.Remove(existingSave);

        var gameSave = new GameSave(
            userId: command.UserId,
            slotId: command.SlotId,
            name: command.Name,
            gameId: command.GameId,
            createdAtUtc: DateTime.UtcNow);
        dbContext.GameSaves.Add(gameSave);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
