using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

internal sealed class LeaguesRoundsCatalog(HybridCache cache, ILeaguesRoundsRepository repository)
    : ILeaguesRoundsCatalog
{
    private const string CacheKey = "leagues:rounds";

    public async ValueTask<IReadOnlyList<LeagueRoundDto>> GetRoundsForLeagueVersionAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
    {
        var cachedRounds = await GetCachedRoundsAsync(cancellationToken).ConfigureAwait(false);
        if (cachedRounds.TryGetValue(leagueVersionId, out var rounds))
            return rounds;

        rounds = await repository
            .GetForVersionAsync(leagueVersionId, cancellationToken)
            .ConfigureAwait(false);

        var updatedRounds = new Dictionary<int, IReadOnlyList<LeagueRoundDto>>(cachedRounds)
        {
            [leagueVersionId] = rounds
        };
        await cache.SetAsync(
            CacheKey,
            updatedRounds,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return rounds;
    }

    private ValueTask<IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>>> GetCachedRoundsAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            CacheKey,
            async cancellationToken => await repository.GetActiveAsync(cancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken);
}
