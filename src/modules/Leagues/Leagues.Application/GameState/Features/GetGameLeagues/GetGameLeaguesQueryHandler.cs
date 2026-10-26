using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagues;

public sealed class GetGameLeaguesQueryHandler(
    LeaguesDbContext dbContext,
    ILeaguesCatalog leaguesCatalog)
{
    public async Task<IReadOnlyList<LeagueDto>> HandleAsync(
        GetGameLeaguesQuery query,
        CancellationToken cancellationToken)
    {
        List<int> gameLeagueVersionIds = await dbContext.GameLeagues
            .AsNoTracking()
            .Where(gameLeague =>
                gameLeague.UserId == query.UserId
                && gameLeague.GameId == query.GameId)
            .Select(gameLeague => gameLeague.LeagueVersionId)
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return await leaguesCatalog.GetLeaguesByVersionIdsAsync(gameLeagueVersionIds, cancellationToken).ConfigureAwait(false);
    }
}
