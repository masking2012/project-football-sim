using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;
using ProjectFootballSim.Teams.Infrastructure.Database;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamById;

public sealed class GetTeamByIdFeature(TeamsDbContext dbContext)
{
    public async Task<TeamSummaryDto?> HandleAsync(int teamId, CancellationToken cancellationToken)
    {
        var team = await dbContext.Teams
            .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken).ConfigureAwait(false);
        return team is null ? null : new TeamSummaryDto
        {
            Id = team.Id,
            Name = team.Name,
            Attack = team.Attack.Value,
            Defence = team.Defence.Value,
            Midfield = team.Midfield.Value
        };
    }
}
