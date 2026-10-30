using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.GameState.Interfaces;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagues;

public sealed class GetGameLeaguesQueryHandler(
    IGameLeaguesService gameLeagueCatalog)
{
    public async Task<IReadOnlyList<LeagueDto>> HandleAsync(
        GetGameLeaguesQuery query,
        CancellationToken cancellationToken) => await gameLeagueCatalog
            .GetGameLeaguesAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false);
}
