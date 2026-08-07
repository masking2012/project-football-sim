using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Application.Common.Services;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;

public sealed class GetTeamsByCountryQueryHandler(ITeamsCatalog teamsCatalog)
{
    public ValueTask<IReadOnlyList<TeamDto>> HandleAsync(
        int countryId,
        CancellationToken cancellationToken) =>
        teamsCatalog.GetByCountryAsync(countryId, cancellationToken);
}
