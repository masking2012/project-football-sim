using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Team.Domain.Entities;
using ProjectFootballSim.Team.Infrastructure.Database;

namespace ProjectFootballSim.Team.Application.Features;

public sealed class GetTeamsByCountryFeature(TeamDbContext dbContext)
{
    public async Task<IEnumerable<TeamEntity>> HandleAsync(int countryId, CancellationToken cancellationToken)
    {
        var teams = await dbContext.Teams
            .Where(t => t.CountryId == countryId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return teams;
    }
}
