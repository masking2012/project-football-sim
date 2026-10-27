using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;

namespace ProjectFootballSim.Leagues.Application.Catalog.Features.GetLeagueByVersionId;

public sealed class GetLeagueByVersionIdQueryHandler(ICachedLeaguesRetriever leaguesRetriever)
{
    public async ValueTask<LeagueDto?> HandleAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
            => await leaguesRetriever.GetLeagueAsync(leagueVersionId, cancellationToken).ConfigureAwait(false);
}
