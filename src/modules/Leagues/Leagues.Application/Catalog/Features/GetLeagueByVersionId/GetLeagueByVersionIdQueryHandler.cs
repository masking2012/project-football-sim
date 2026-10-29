using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Features.GetLeagueByVersionId;

public sealed class GetLeagueByVersionIdQueryHandler(ICachedLeaguesRetriever leaguesRetriever)
{
    public async ValueTask<LeagueDto?> HandleAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
            => await leaguesRetriever.GetLeagueAsync(leagueVersionId, cancellationToken).ConfigureAwait(false);
}
