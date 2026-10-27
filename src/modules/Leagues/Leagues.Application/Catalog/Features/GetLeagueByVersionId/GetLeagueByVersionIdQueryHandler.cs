using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;

namespace ProjectFootballSim.Leagues.Application.Catalog.Features.GetLeagueByVersionId;

public sealed class GetLeagueByVersionIdQueryHandler(ICachedLeaguesRetriever leagueCatalog)
{
    public async ValueTask<LeagueDto?> HandleAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
            => await leagueCatalog.GetLeagueAsync(leagueVersionId, cancellationToken).ConfigureAwait(false);
}
