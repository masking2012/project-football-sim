using ProjectFootballSim.Teams.Application.Common.Interfaces;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByCountry;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;

internal sealed class GetTeamsByCountryQueryHandler(ICacheTeamsRetriever teamsRetriever)
    : IGetTeamsByCountryQueryHandler
{
    public async Task<IReadOnlyDictionary<int, TeamByCountryDto>> HandleAsync(
        int countryId,
        CancellationToken cancellationToken)
    {
        var countryTeams = await teamsRetriever.GetByCountryAsync(countryId, cancellationToken).ConfigureAwait(false);
        return countryTeams.ToDictionary(t => t.Id, t => new TeamByCountryDto
        (
            Id: t.Id,
            Name: t.Name,
            Attack: t.Attack,
            Midfield: t.Midfield,
            Defence: t.Defence
        ));
    }
}
