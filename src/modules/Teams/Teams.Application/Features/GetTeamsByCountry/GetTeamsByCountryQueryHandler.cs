using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Infrastructure.Database;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;

public sealed class GetTeamsByCountryQueryHandler(TeamsDbContext dbContext)
{
    public async Task<IEnumerable<TeamDto>> HandleAsync(int countryId, CancellationToken cancellationToken)
    {
        var teams = await dbContext.Teams
            .Where(t => t.CountryId == countryId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return teams.Select(t => new TeamDto(
            Id: t.Id,
            Name: t.Name,
            Attack: t.Attack.Value,
            Midfield: t.Midfield.Value,
            Defence: t.Defence.Value));
    }
}
