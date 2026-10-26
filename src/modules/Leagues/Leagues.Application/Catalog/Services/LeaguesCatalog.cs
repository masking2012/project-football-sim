using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Infrastructure.Database;
using LeagueDomain = ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects.League;
using LeagueVersionDomain = ProjectFootballSim.Leagues.Domain.Catalog.Entities.LeagueVersion;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

internal sealed class LeaguesCatalog(HybridCache cache, LeaguesDbContext dbContext) : ILeaguesCatalog
{
    private const string CacheKey = "leagues";

    public async ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllActiveAsync(
        CancellationToken cancellationToken)
    {
        Dictionary<int, LeagueDto> cachedLeagues
            = await GetCachedLeaguesAsync(cancellationToken).ConfigureAwait(false);
        return cachedLeagues
            .Where(kvp => kvp.Value.IsActive)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public async ValueTask<LeagueDto?> GetByLeagueVersionIdAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
    {
        var cachedLeagues = await GetCachedLeaguesAsync(cancellationToken).ConfigureAwait(false);
        if (cachedLeagues.TryGetValue(leagueVersionId, out var league))
            return league;

        league = await LoadLeagueVersionAsync(leagueVersionId, cancellationToken).ConfigureAwait(false);
        if (league is null)
            return null;

        var updatedLeagues = new Dictionary<int, LeagueDto>(cachedLeagues)
        {
            [league.LeagueVersionId] = league
        };
        await cache.SetAsync(
            CacheKey,
            updatedLeagues,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return league;
    }

    private ValueTask<Dictionary<int, LeagueDto>> GetCachedLeaguesAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            CacheKey,
            async cancellationToken => await LoadActiveLeaguesAsync(cancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken);

    private async Task<Dictionary<int, LeagueDto>> LoadActiveLeaguesAsync(
        CancellationToken cancellationToken)
    {
        var activeLeagues = await dbContext.Leagues
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
            .Where(item => item.activation.LeagueVersionId == item.version.LeagueVersionId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var leaguesById = activeLeagues
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
            .ToDictionary(league => league.LeagueVersionId);

        return leaguesById;
    }

    private async Task<LeagueDto?> LoadLeagueVersionAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
    {
        var version = await dbContext.LeagueVersions
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.LeagueVersionId == leagueVersionId, cancellationToken)
            .ConfigureAwait(false);

        if (version is null)
            return null;

        var league = await dbContext.Leagues
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == version.LeagueId, cancellationToken)
            .ConfigureAwait(false);

        if (league is null)
            return null;

        var activeLeagueVersionId = await dbContext.LeagueActivations
            .AsNoTracking()
            .Where(item => item.LeagueId == version.LeagueId)
            .Select(item => (int?)item.LeagueVersionId)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return CreateLeagueDto(league, version, activeLeagueVersionId);
    }

    private static LeagueDto CreateLeagueDto(
        LeagueDomain league,
        LeagueVersionDomain version,
        int? activeLeagueVersionId) =>
        new(
            league.Id,
            version.LeagueVersionId,
            version.Version,
            IsActive: activeLeagueVersionId == version.LeagueVersionId,
            league.Name,
            league.Order,
            league.CountryId,
            version.TeamsCount,
            version.Format
                .OrderBy(stage => stage.Order)
                .Select(stage => new LeagueFormatDto(stage.Order, (LeagueStageTypeDto)stage.Type, stage.Legs))
                .ToList(),
            version.PromotionPositions,
            version.PromotionPlayOffPositions,
            version.RelegationPositions,
            version.RelegationPlayOffPositions,
            version.UefaChampionsLeaguePositions,
            version.UefaEuropaLeaguePositions,
            version.UefaConferenceLeaguePositions);
}
