using ProjectFootballSim.Teams.Application.Common.Interfaces;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamsByIds;

internal sealed class GetTeamsByIdsQueryHandler(ICacheTeamsRetriever teamsCatalog) : IGetTeamsByIdsQueryHandler
{
    public async Task<IReadOnlyDictionary<int, TeamByIdsDto>> HandleAsync(
        IReadOnlyList<int> teamIds, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(teamIds);
        if (teamIds.Count == 0)
            throw new ArgumentException("The list of team ids cannot be empty.", nameof(teamIds));
        if (teamIds.Count != teamIds.Distinct().Count())
            throw new ArgumentException("The list of team ids cannot contain duplicates.", nameof(teamIds));

        var teams = await teamsCatalog.GetByIdsAsync(teamIds, cancellationToken).ConfigureAwait(false);
        if (teams.Count != teamIds.Count)
        {
            var missingTeamIds = teamIds.Except(teams.Select(t => t.Id)).ToList();
            throw new InvalidOperationException(
                $"The following team IDs were not found in the catalog: {string.Join(", ", missingTeamIds)}");
        }

        return teams.Select(t => new TeamByIdsDto
        (
            Id: t.Id,
            Name: t.Name,
            Attack: t.Attack,
            Midfield: t.Midfield,
            Defence: t.Defence,
            CountryId: t.CountryId
        )).ToDictionary(t => t.Id);
    }
}
