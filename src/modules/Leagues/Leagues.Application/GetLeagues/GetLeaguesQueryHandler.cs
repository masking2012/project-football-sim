using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Leagues.Application.Common.Models;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GetLeagues;

public sealed class GetLeaguesQueryHandler(HybridCache cache, LeaguesDbContext dbContext)
{
    public ValueTask<IReadOnlyDictionary<int, LeagueDto>> HandleAsync(CancellationToken cancellationToken)
    {
        return cache.GetOrCreateAsync(
            "leagues",
            async lambdaCancellationToken => await GetDataFromTheSourceAsync(lambdaCancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken
        );
    }

    private async Task<IReadOnlyDictionary<int, LeagueDto>> GetDataFromTheSourceAsync( CancellationToken cancellationToken)
    {
        var leagues = await dbContext.Leagues
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return leagues
            .Select(league => new LeagueDto(league.Id, league.Name, league.Order, league.CountryId))
            .ToDictionary(league => league.Id);
    }
}
