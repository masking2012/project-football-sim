using ProjectFootballSim.Teams.Application.Common.Models;

namespace ProjectFootballSim.Teams.Application.Common.Interfaces;

public interface ICacheTeamsRetriever
{
    ValueTask<IReadOnlyList<TeamDto>> GetByIdsAsync(
        IReadOnlyList<int> teamIds, CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<TeamDto>> GetByCountryAsync(
        int countryId, CancellationToken cancellationToken);
}
