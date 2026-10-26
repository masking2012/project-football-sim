using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GameState.Services;

internal sealed class GameLeagueCatalog(
    LeaguesDbContext dbContext,
    ILeaguesCatalog leaguesCatalog) : IGameLeagueCatalog
{
    public async Task<IReadOnlyList<LeagueDto>> GetByGameAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken)
    {
        List<int> leagueVersionIds = await dbContext.GameLeagues
            .AsNoTracking()
            .Where(gameLeague =>
                gameLeague.UserId == userId
                && gameLeague.GameId == gameId)
            .Select(gameLeague => gameLeague.LeagueVersionId)
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return await leaguesCatalog
            .GetLeaguesByVersionIdsAsync(leagueVersionIds, cancellationToken)
            .ConfigureAwait(false);
    }
}
