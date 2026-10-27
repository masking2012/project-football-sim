using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

internal sealed class LeaguesCatalog(HybridCache cache, ILeaguesRepository repository) : ILeaguesCatalog
{
    private const string CacheKey = "leagues";

    public async ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllActiveAsync(
        CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<int, LeagueDto> cachedLeagues
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

        league = await repository
            .GetByLeagueVersionIdAsync(leagueVersionId, cancellationToken)
            .ConfigureAwait(false);
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

    public async ValueTask<IReadOnlyList<LeagueDto>> GetLeaguesByVersionIdsAsync(
        IReadOnlyCollection<int> leagueVersionIds,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(leagueVersionIds);

        var leagues = new List<LeagueDto>(leagueVersionIds.Count);
        foreach (var leagueVersionId in leagueVersionIds.Distinct())
        {
            var league = await GetByLeagueVersionIdAsync(leagueVersionId, cancellationToken)
                .ConfigureAwait(false);

            if (league is null)
                throw new InvalidOperationException($"League version with ID '{leagueVersionId}' was not found.");

            leagues.Add(league);
        }

        return leagues;
    }

    private ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetCachedLeaguesAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            CacheKey,
            async cancellationToken => await repository.GetActiveAsync(cancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken);
}
