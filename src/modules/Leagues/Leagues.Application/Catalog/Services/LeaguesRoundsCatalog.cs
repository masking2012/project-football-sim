using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

internal sealed class LeaguesRoundsCatalog(HybridCache cache, LeaguesDbContext dbContext)
    : ILeaguesRoundsCatalog
{
    private const string CacheKey = "leagues:rounds";

    public async ValueTask<IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
    }

    private ValueTask<IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>>> GetCatalogAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            CacheKey,
            async cancellationToken => await LoadCatalogAsync(cancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken);

    private async Task<IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>>> LoadCatalogAsync(
        CancellationToken cancellationToken)
    {
        var leagueRounds = await dbContext.LeagueRounds
            .AsNoTracking()
            .Join(
                dbContext.LeagueActivations.AsNoTracking(),
                round => round.LeagueVersionId,
                activation => activation.LeagueVersionId,
                (round, activation) => new { round, activation })
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                item => item.round.LeagueVersionId,
                version => version.LeagueVersionId,
                (item, version) => new { item.round, item.activation, version })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var leagueRoundsByLeagueId = leagueRounds
            .GroupBy(leagueRound => leagueRound.version.LeagueId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<LeagueRoundDto>)group
                    .Select(leagueRound => new LeagueRoundDto(
                        leagueRound.version.LeagueId,
                        leagueRound.round.LeagueVersionId,
                        leagueRound.round.Round,
                        leagueRound.round.Week,
                        leagueRound.round.IsMidweek))
                    .ToList());

        return leagueRoundsByLeagueId;
    }
}
