using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;

namespace ProjectFootballSim.Leagues.Application.GameState.Services;

internal sealed class GameLeagueCatalog(
    IGameLeagueRepository repository,
    ICachedLeaguesRetriever leaguesCatalog) : IGameLeagueCatalog
{
    public async Task<IReadOnlyList<LeagueDto>> GetByGameAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<int> leagueVersionIds = await repository
            .GetLeagueVersionIdsAsync(userId, gameId, cancellationToken)
            .ConfigureAwait(false);

        return await leaguesCatalog
            .GetLeaguesAsync(leagueVersionIds, cancellationToken)
            .ConfigureAwait(false);
    }
}
