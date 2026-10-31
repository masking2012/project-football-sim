using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.GameState.Interfaces;

namespace ProjectFootballSim.Leagues.Application.GameState.Services;

internal sealed class GameLeagueService(
    IGameLeaguesRepository repository,
    ICachedLeaguesRetriever leaguesRetriever) : IGameLeaguesService
{
    public async Task<IReadOnlyList<LeagueDto>> GetGameLeaguesAsync(
        Guid userId,
        Guid gameId,
        CancellationToken cancellationToken)
    {
        //TODO: think whether we need this class

        IReadOnlyList<int> leagueVersionIds = await repository
            .GetLeagueVersionIdsAsync(userId, gameId, cancellationToken)
            .ConfigureAwait(false);

        var result = await leaguesRetriever
            .GetLeaguesAsync(leagueVersionIds, cancellationToken)
            .ConfigureAwait(false);

        return result.Values.ToList();
    }
}
