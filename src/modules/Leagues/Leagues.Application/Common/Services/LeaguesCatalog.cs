using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Leagues.Application.Common.Models;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.Common.Services;

internal sealed class LeaguesCatalog(HybridCache cache, LeaguesDbContext dbContext) : ILeaguesCatalog
{
    private const string CacheKey = "leagues";

    public async ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var catalog = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return catalog.LeaguesById;
    }

    public async ValueTask<LeagueDto?> GetByIdAsync(
        int leagueId,
        CancellationToken cancellationToken)
    {
        var catalog = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return catalog.LeaguesById.GetValueOrDefault(leagueId);
    }

    private ValueTask<LeaguesCatalogSnapshot> GetCatalogAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            CacheKey,
            async cancellationToken => await LoadCatalogAsync(cancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken);

    private async Task<LeaguesCatalogSnapshot> LoadCatalogAsync(
        CancellationToken cancellationToken)
    {
        var leagues = await dbContext.Leagues
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var leaguesById = leagues
            .Select(league => new LeagueDto(
                league.Id,
                league.Name,
                league.Order,
                league.CountryId))
            .ToDictionary(league => league.Id);

        return new LeaguesCatalogSnapshot(leaguesById);
    }
}
