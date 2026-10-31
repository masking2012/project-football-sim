using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Shared.Features.EntityFrameworkCore;

namespace ProjectFootballSim.Calendar.Infrastructure.Repositories;

internal sealed class CreateGameSeasonRepository(CalendarDbContext dbContext) : ICreateGameSeasonRepository
{
    public async Task<GameSeason?> GetCurrentSeasonAsync(Guid userId, Guid gameId, CancellationToken cancellationToken)
    {
        GameSeason? currentSeason = await dbContext.GameSeasons
            .SingleOrDefaultAsync(s => s.UserId == userId && s.GameId == gameId && s.IsCurrent, cancellationToken)
            .ConfigureAwait(false);
        return currentSeason;
    }

    public void Add(GameSeason gameSeason)
    {
        dbContext.GameSeasons.Add(gameSeason);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new GameSeasonExistsException("A game season already exists.", ex);
        }        
    }
}
