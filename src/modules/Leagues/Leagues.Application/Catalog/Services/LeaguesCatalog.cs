using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

internal sealed class LeaguesCatalog(HybridCache cache, LeaguesDbContext dbContext) : ILeaguesCatalog
{
    private const string CacheKey = "leagues";

    public async ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var catalog = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return catalog.LeaguesById;
    }

    public async ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllActiveAsync(
        CancellationToken cancellationToken)
    {
        var allLeagues = await GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allLeagues.Where(kvp => kvp.Value.IsActive).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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
            .Join(
                dbContext.LeagueActivations.AsNoTracking(),
                league => league.Id,
                activation => activation.LeagueId,
                (league, activation) => new { league, activation })
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                item => item.activation.LeagueVersionId,
                version => version.LeagueVersionId,
                (item, version) => new { item.league, item.activation, version })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var leaguesById = leagues
            .Select(item => new LeagueDto(
                item.league.Id,
                item.version.LeagueVersionId,
                item.version.Version,
                IsActive: item.activation.LeagueVersionId == item.version.LeagueVersionId,
                item.league.Name,
                item.league.Order,
                item.league.CountryId,
                item.version.TeamsCount,
                item.version.Format
                    .OrderBy(stage => stage.Order)
                    .Select(stage => new LeagueFormatDto(stage.Order, (LeagueStageTypeDto)stage.Type, stage.Legs))
                    .ToList(),
                item.version.PromotionPositions,
                item.version.PromotionPlayOffPositions,
                item.version.RelegationPositions,
                item.version.RelegationPlayOffPositions,
                item.version.UefaChampionsLeaguePositions,
                item.version.UefaEuropaLeaguePositions,
                item.version.UefaConferenceLeaguePositions))
            .ToDictionary(league => league.Id);

        return new LeaguesCatalogSnapshot(leaguesById);
    }
}
