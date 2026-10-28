using ProjectFootballSim.Teams.Application.Common.Models;

namespace ProjectFootballSim.Teams.Application.Common.Interfaces;

public interface ICacheTeamsRetriever
{
    ValueTask<TeamDto?> GetByIdAsync(int teamId, CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<TeamDto>> GetByCountryAsync(
        int countryId,
        CancellationToken cancellationToken);
}
