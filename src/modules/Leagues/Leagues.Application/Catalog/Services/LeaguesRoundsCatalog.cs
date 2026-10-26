using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Infrastructure.Database;
using LeagueRoundDomain = ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects.LeagueRound;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

internal sealed class LeaguesRoundsCatalog(HybridCache cache, LeaguesDbContext dbContext)
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

        rounds = await LoadRoundsForVersionAsync(leagueVersionId, cancellationToken).ConfigureAwait(false);

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

    private ValueTask<Dictionary<int, IReadOnlyList<LeagueRoundDto>>> GetCachedRoundsAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            CacheKey,
            async cancellationToken => await LoadActiveRoundsAsync(cancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken);

    private async Task<Dictionary<int, IReadOnlyList<LeagueRoundDto>>> LoadActiveRoundsAsync(
        CancellationToken cancellationToken)
    {
        var activeRounds = await dbContext.LeagueRounds
            .AsNoTracking()
            .Join(
                dbContext.LeagueActivations.AsNoTracking(),
                round => round.LeagueVersionId,
                activation => activation.LeagueVersionId,
                (round, activation) => round)
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                round => round.LeagueVersionId,
                version => version.LeagueVersionId,
                (round, version) => new { round, version })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return activeRounds
            .GroupBy(item => item.round.LeagueVersionId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<LeagueRoundDto>)group
                    .Select(item => CreateRoundDto(item.version.LeagueId, item.round))
                    .ToList());
    }

    private async Task<IReadOnlyList<LeagueRoundDto>> LoadRoundsForVersionAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
    {
        var rounds = await dbContext.LeagueRounds
            .AsNoTracking()
            .Where(round => round.LeagueVersionId == leagueVersionId)
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                round => round.LeagueVersionId,
                version => version.LeagueVersionId,
                (round, version) => new { round, version })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rounds
            .Select(item => CreateRoundDto(item.version.LeagueId, item.round))
            .ToList();
    }

    private static LeagueRoundDto CreateRoundDto(int leagueId, LeagueRoundDomain round) =>
        new(leagueId, round.LeagueVersionId, round.Round, round.Week, round.IsMidweek);
}
