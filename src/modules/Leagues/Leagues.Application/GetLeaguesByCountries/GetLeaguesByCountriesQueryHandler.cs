using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GetLeaguesByCountries;

public sealed class GetLeaguesByCountriesQueryHandler(LeaguesDbContext dbContext)
{
    public async Task<IReadOnlyList<LeagueDto>> HandleAsync(GetLeaguesByCountriesQuery query, CancellationToken cancellationToken)
    {
        var leagues = await dbContext.Leagues
            .Where(league => query.CountryIds.Contains(league.CountryId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return leagues
            .Select(league => new LeagueDto(league.Id, league.Name, league.Order, league.CountryId))
            .ToList();
    }
}
