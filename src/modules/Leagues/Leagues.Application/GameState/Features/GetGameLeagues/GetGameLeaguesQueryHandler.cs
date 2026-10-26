using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.GameState.Services;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagues;

public sealed class GetGameLeaguesQueryHandler(
    IGameLeagueCatalog gameLeagueCatalog)
{
    public async Task<IReadOnlyList<LeagueDto>> HandleAsync(
        GetGameLeaguesQuery query,
        CancellationToken cancellationToken) => await gameLeagueCatalog
            .GetByGameAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false);
}
