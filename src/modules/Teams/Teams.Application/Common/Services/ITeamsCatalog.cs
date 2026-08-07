using ProjectFootballSim.Teams.Application.Common.Models;

namespace ProjectFootballSim.Teams.Application.Common.Services;

public interface ITeamsCatalog
{
    ValueTask<TeamDto?> GetByIdAsync(int teamId, CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<TeamDto>> GetByCountryAsync(
        int countryId,
        CancellationToken cancellationToken);
}
