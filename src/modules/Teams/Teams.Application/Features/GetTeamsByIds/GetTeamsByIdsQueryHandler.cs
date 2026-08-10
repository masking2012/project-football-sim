using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Application.Common.Services;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamsByIds;

public sealed class GetTeamsByIdsQueryHandler(ITeamsCatalog teamsCatalog)
{
    public async ValueTask<IReadOnlyDictionary<int, TeamDto>> HandleAsync(IReadOnlyList<int> teamIds, CancellationToken cancellationToken)
    {
        var result = new Dictionary<int, TeamDto>();
        foreach (var teamId in teamIds.Distinct())
        {
            var team = await teamsCatalog.GetByIdAsync(teamId, cancellationToken).ConfigureAwait(false);
            if (team != null)
                result[teamId] = team;
        }
        return result;
    }
}
