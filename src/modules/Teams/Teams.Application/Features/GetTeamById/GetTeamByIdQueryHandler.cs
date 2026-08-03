using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Infrastructure.Database;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamById;

public sealed class GetTeamByIdQueryHandler(TeamsDbContext dbContext)
{
    public async Task<TeamDto?> HandleAsync(int teamId, CancellationToken cancellationToken)
    {
        var team = await dbContext.Teams
            .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken).ConfigureAwait(false);
        return team is null
            ? null
            : new TeamDto(
                Id: team.Id,
                Name: team.Name,
                Attack: team.Attack.Value,
                Midfield: team.Midfield.Value,
                Defence: team.Defence.Value);
    }
}
