using ProjectFootballSim.Teams.Application.Common.Interfaces;
using ProjectFootballSim.Teams.Application.Common.Models;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamById;

public sealed class GetTeamByIdQueryHandler(ICacheTeamsRetriever teamsCatalog)
{
    public ValueTask<TeamDto?> HandleAsync(
        int teamId,
        CancellationToken cancellationToken) =>
        teamsCatalog.GetByIdAsync(teamId, cancellationToken);
}
