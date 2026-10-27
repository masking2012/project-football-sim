using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class LeaguesTeamsRepository(LeaguesDbContext dbContext) : ILeaguesTeamsRepository
{
    public async Task<IReadOnlyList<int>> GetTeamIdsAsync(
        int leagueVersionId,
        CancellationToken cancellationToken) =>
            await dbContext.LeagueTeams
                .AsNoTracking()
                .Where(team => team.LeagueVersionId == leagueVersionId)
                .Select(team => team.TeamId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
}
