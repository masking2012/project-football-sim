using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Infrastructure.Database;

namespace ProjectFootballSim.Teams.Application.Common.Services;

internal sealed class TeamsCatalog(HybridCache cache, TeamsDbContext dbContext) : ITeamsCatalog
{
    private const string CacheKey = "teams:catalog";

    public async ValueTask<TeamDto?> GetByIdAsync(
        int teamId,
        CancellationToken cancellationToken)
    {
        var catalog = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return catalog.TeamsById.GetValueOrDefault(teamId);
    }

    public async ValueTask<IReadOnlyList<TeamDto>> GetByCountryAsync(
        int countryId,
        CancellationToken cancellationToken)
    {
        var catalog = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);

        if (!catalog.TeamIdsByCountry.TryGetValue(countryId, out var teamIds))
            return Array.Empty<TeamDto>();

        return teamIds
            .Select(teamId => catalog.TeamsById[teamId])
            .ToArray();
    }

    private ValueTask<TeamsCatalogSnapshot> GetCatalogAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            CacheKey,
            async cancellationToken => await LoadCatalogAsync(cancellationToken).ConfigureAwait(false),
            cancellationToken: cancellationToken);

    private async Task<TeamsCatalogSnapshot> LoadCatalogAsync(
        CancellationToken cancellationToken)
    {
        var teams = await dbContext.Teams
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var teamsById = teams.ToDictionary(
            team => team.Id,
            team => new TeamDto(
                Id: team.Id,
                Name: team.Name,
                Attack: team.Attack.Value,
                Midfield: team.Midfield.Value,
                Defence: team.Defence.Value));

        var teamIdsByCountry = teams
            .GroupBy(team => team.CountryId)
            .ToDictionary(
                group => group.Key,
                group => group.Select(team => team.Id).ToArray());

        return new TeamsCatalogSnapshot(teamsById, teamIdsByCountry);
    }
}
