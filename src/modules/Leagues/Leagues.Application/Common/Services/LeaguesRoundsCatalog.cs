using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Leagues.Application.Common.Models;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.Common.Services;

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
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var leagueRoundsByLeagueId = leagueRounds
            .GroupBy(leagueRound => leagueRound.LeagueId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<LeagueRoundDto>)group
                    .Select(leagueRound => new LeagueRoundDto(
                        leagueRound.LeagueId,
                        leagueRound.Round,
                        leagueRound.Week,
                        leagueRound.IsMidweek))
                    .ToList());

        return leagueRoundsByLeagueId;
    }
}
