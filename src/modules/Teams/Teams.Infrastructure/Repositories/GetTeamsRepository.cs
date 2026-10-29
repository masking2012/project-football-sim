using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Teams.Application.Common.Interfaces;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Infrastructure.Database;

namespace ProjectFootballSim.Teams.Infrastructure.Repositories;

internal sealed class GetTeamsRepository(TeamsDbContext dbContext) : IGetTeamsRepository
{
    public async Task<IReadOnlyList<TeamDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Teams
            .AsNoTracking()
            .Select(team => new TeamDto(
                Id: team.Id,
                Name: team.Name,
                Attack: team.Attack.Value,
                Midfield: team.Midfield.Value,
                Defence: team.Defence.Value,
                CountryId: team.CountryId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
