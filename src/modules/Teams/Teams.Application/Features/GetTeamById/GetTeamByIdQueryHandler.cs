using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Application.Common.Services;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamById;

public sealed class GetTeamByIdQueryHandler(ITeamsCatalog teamsCatalog)
{
    public ValueTask<TeamDto?> HandleAsync(
        int teamId,
        CancellationToken cancellationToken) =>
        teamsCatalog.GetByIdAsync(teamId, cancellationToken);
}
