using Microsoft.Extensions.Caching.Hybrid;
using ProjectFootballSim.Teams.Application.Common.Interfaces;
using ProjectFootballSim.Teams.Application.Common.Models;

namespace ProjectFootballSim.Teams.Application.Common.Services;

internal sealed class CacheTeamsRetriever(HybridCache cache, IGetTeamsRepository teamsRepository) : ICacheTeamsRetriever
{
    private const string CacheKey = "teams";

    public async ValueTask<TeamDto?> GetByIdAsync(
        int teamId,
        CancellationToken cancellationToken)
    {
        var cachedTeams = await GetAllCachedTeamsAsync(cancellationToken).ConfigureAwait(false);
        return cachedTeams.GetValueOrDefault(teamId);
    }

    public async ValueTask<IReadOnlyList<TeamDto>> GetByCountryAsync(
        int countryId,
        CancellationToken cancellationToken)
    {
        var cachedTeams = await GetAllCachedTeamsAsync(cancellationToken).ConfigureAwait(false);

        return cachedTeams.Values
            .Where(team => team.CountryId == countryId)
            .ToList();
    }

    private ValueTask<IReadOnlyDictionary<int, TeamDto>> GetAllCachedTeamsAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            CacheKey,
            LoadTeamsAsync,
            cancellationToken: cancellationToken);

    private async ValueTask<IReadOnlyDictionary<int, TeamDto>> LoadTeamsAsync(
        CancellationToken cancellationToken)
    {
        var teams = await teamsRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return teams.ToDictionary(team => team.Id);
    }
}
