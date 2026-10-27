using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

internal sealed class CachedLeaguesRetriever(
    HybridCache cache,
    ILeaguesRetriever leaguesRetriever,
    ILeaguesRoundsRetriever leaguesRoundsRetriever)
    : ICachedLeaguesRetriever
{
    private const string CacheKey = "leagues";

    public async ValueTask<IReadOnlyDictionary<int, LeagueDto>> GetAllActiveLeaguesAsync(
        CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<int, LeagueDto> cachedLeagues
            = (await GetCachedLeaguesAsync(cancellationToken).ConfigureAwait(false))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.League);
        return cachedLeagues
            .Where(kvp => kvp.Value.IsActive)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public async ValueTask<LeagueDto?> GetLeagueAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
    {
        var cachedLeagues = await GetCachedLeaguesAsync(cancellationToken).ConfigureAwait(false);
        if (cachedLeagues.TryGetValue(leagueVersionId, out var snapshot))
            return snapshot.League;

        LeagueDto? leagueDto = await leaguesRetriever
            .GetAsync(leagueVersionId, cancellationToken)
            .ConfigureAwait(false);
        if (leagueDto is null)
            return null;

        IReadOnlyList<LeagueRoundDto> roundsDto = await leaguesRoundsRetriever.GetAsync(leagueVersionId, cancellationToken)
            .ConfigureAwait(false);
        if (roundsDto is null || roundsDto.Count == 0)
            throw new InvalidDataException($"League rounds for league version with ID '{leagueVersionId}' were not found.");

        await SaveInCacheAsync(cachedLeagues, leagueDto, roundsDto, cancellationToken)
            .ConfigureAwait(false);

        return leagueDto;
    }

    public async ValueTask<IReadOnlyList<LeagueDto>> GetLeaguesAsync(
        IReadOnlyCollection<int> leagueVersionIds,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(leagueVersionIds);
        if (leagueVersionIds.Count != leagueVersionIds.Distinct().Count())
            throw new ArgumentException(
                "League version IDs must not contain duplicates.",
                nameof(leagueVersionIds));

        var leagues = new List<LeagueDto>(leagueVersionIds.Count);
        foreach (var leagueVersionId in leagueVersionIds)
        {
            var league = await GetLeagueAsync(leagueVersionId, cancellationToken)
                .ConfigureAwait(false);

            if (league is null)
                throw new InvalidOperationException($"League version with ID '{leagueVersionId}' was not found.");

            leagues.Add(league);
        }

        return leagues;
    }

    public async ValueTask<IReadOnlyList<LeagueRoundDto>> GetLeagueRoundsAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<int, LeagueSnapshot> cachedLeagues
            = await GetCachedLeaguesAsync(cancellationToken).ConfigureAwait(false);

        if (cachedLeagues.TryGetValue(leagueVersionId, out var snapshot))
            return snapshot.Rounds;

        LeagueDto? leagueDto = await leaguesRetriever
            .GetAsync(leagueVersionId, cancellationToken)
            .ConfigureAwait(false);
        if (leagueDto is null)
            throw new InvalidDataException($"League version with ID '{leagueVersionId}' was not found.");

        IReadOnlyList<LeagueRoundDto> roundsDto = await leaguesRoundsRetriever.GetAsync(leagueVersionId, cancellationToken)
            .ConfigureAwait(false);
        if (roundsDto is null || roundsDto.Count == 0)
            throw new InvalidDataException($"League rounds for league version with ID '{leagueVersionId}' were not found.");

        await SaveInCacheAsync(cachedLeagues, leagueDto, roundsDto, cancellationToken)
            .ConfigureAwait(false);

        return roundsDto;
    }

    private ValueTask<Dictionary<int, LeagueSnapshot>> GetCachedLeaguesAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            CacheKey,
            async cancellationToken =>
            {
                Dictionary<int, LeagueSnapshot> cachedLeagues = new();

                IReadOnlyDictionary<int, LeagueDto> leagues = await leaguesRetriever
                    .GetAllActiveAsync(cancellationToken)
                    .ConfigureAwait(false);
                IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>> leaguesRounds = await leaguesRoundsRetriever
                    .GetRoundsFromAllActiveVersionsAsync(cancellationToken)
                    .ConfigureAwait(false);
                foreach (var league in leagues)
                {
                    if (!leaguesRounds.TryGetValue(league.Key, out var leagueRounds))
                        throw new InvalidDataException($"League rounds for league version with ID '{league.Key}' were not found.");

                    cachedLeagues[league.Key] = new LeagueSnapshot(
                        League: league.Value,
                        Rounds: leagueRounds
                    );
                }

                return cachedLeagues;
            },
            cancellationToken: cancellationToken);

    private ValueTask SaveInCacheAsync(
        IReadOnlyDictionary<int, LeagueSnapshot> currentCache,
        LeagueDto leagueDto,
        IReadOnlyList<LeagueRoundDto> roundsDto,
        CancellationToken cancellationToken)
    {
        var updatedLeagues = new Dictionary<int, LeagueSnapshot>(currentCache)
        {
            [leagueDto.LeagueVersionId] = new LeagueSnapshot(leagueDto, roundsDto)
        };
        return cache.SetAsync(
            CacheKey,
            updatedLeagues,
            cancellationToken: cancellationToken);
    }

    private sealed record LeagueSnapshot(LeagueDto League, IReadOnlyList<LeagueRoundDto> Rounds);
}
