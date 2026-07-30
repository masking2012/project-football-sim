using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;
using ProjectFootballSim.Teams.Infrastructure.Database;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;

public sealed class GetTeamsByCountryFeature(TeamsDbContext dbContext)
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
