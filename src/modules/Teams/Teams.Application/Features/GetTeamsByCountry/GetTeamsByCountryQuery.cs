using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Infrastructure.Database;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;

public sealed class GetTeamsByCountryQuery(TeamsDbContext dbContext)
{
    public async Task<IEnumerable<TeamDto>> HandleAsync(int countryId, CancellationToken cancellationToken)
    {
        var teams = await dbContext.Teams
            .Where(t => t.CountryId == countryId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return teams.Select(t => new TeamDto(t.Id, t.Name, t.Attack.Value, t.Midfield.Value, t.Defence.Value));
    }
}
