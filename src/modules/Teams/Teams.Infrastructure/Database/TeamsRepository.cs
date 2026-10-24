using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Teams.Application.Common.Services;
using ProjectFootballSim.Teams.Domain.Entities;

namespace ProjectFootballSim.Teams.Infrastructure.Database;

internal sealed class TeamsRepository(TeamsDbContext dbContext) : ITeamsRepository
{
    public async Task<IReadOnlyList<Team>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Teams
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
