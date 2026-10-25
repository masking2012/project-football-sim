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
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var leaguesById = leagues
            .Select(item => new LeagueDto(
                item.league.Id,
                item.league.Version,
                IsActive: item.activation.CurrentVersion == item.league.Version,
                item.league.Name,
                item.league.Order,
                item.league.CountryId,
                item.league.TeamsCount,
                item.league.Format
                    .OrderBy(stage => stage.Order)
                    .Select(stage => new LeagueFormatDto(stage.Order, (LeagueStageTypeDto)stage.Type, stage.Legs))
                    .ToList(),
                item.league.PromotionPositions,
                item.league.PromotionPlayOffPositions,
                item.league.RelegationPositions,
                item.league.RelegationPlayOffPositions,
                item.league.UefaChampionsLeaguePositions,
                item.league.UefaEuropaLeaguePositions,
                item.league.UefaConferenceLeaguePositions))
            .ToDictionary(league => league.Id);

        return new LeaguesCatalogSnapshot(leaguesById);
    }
}
