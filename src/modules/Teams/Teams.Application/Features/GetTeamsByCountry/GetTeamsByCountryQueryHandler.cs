using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Infrastructure.Database;

namespace ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;

public sealed class GetTeamsByCountryQueryHandler(HybridCache cache, TeamsDbContext dbContext)
{
    public ValueTask<IEnumerable<TeamDto>> HandleAsync(int countryId, CancellationToken cancellationToken)
    {
        return cache.GetOrCreateAsync(
            $"teams_{countryId}",
            async lambdaCancellationToken => await GetDataFromTheSourceAsync(countryId, lambdaCancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken
        );
    }

    private async Task<IEnumerable<TeamDto>> GetDataFromTheSourceAsync(int countryId, CancellationToken cancellationToken)
    {
        var teams = await dbContext.Teams
            .Where(t => t.CountryId == countryId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return teams.Select(t => new TeamDto(
            Id: t.Id,
            Name: t.Name,
            Attack: t.Attack.Value,
            Midfield: t.Midfield.Value,
            Defence: t.Defence.Value));
    }
}
