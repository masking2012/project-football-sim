using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Team.Infrastructure.Database;

namespace ProjectFootballSim.Team.Application.Features.GetTeamsByCountry;

public sealed class GetTeamsByCountryFeature(TeamDbContext dbContext)
{
    public async Task<IEnumerable<TeamSummaryDto>> HandleAsync(int countryId, CancellationToken cancellationToken)
    {
        var teams = await dbContext.Teams
            .Where(t => t.CountryId == countryId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return teams.Select(t => new TeamSummaryDto
        {
            Id = t.Id,
            Name = t.Name,
            Attack = t.Attack.Value,
            Defence = t.Defence.Value,
            Midfield = t.Midfield.Value,
        });
    }
}
