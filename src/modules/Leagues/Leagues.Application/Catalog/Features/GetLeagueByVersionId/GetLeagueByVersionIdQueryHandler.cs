using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;

namespace ProjectFootballSim.Leagues.Application.Catalog.Features.GetLeagueByVersionId;

public sealed class GetLeagueByVersionIdQueryHandler(ILeaguesCatalog leagueCatalog)
{
    public async ValueTask<LeagueDto?> HandleAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
            => await leagueCatalog.GetByLeagueVersionIdAsync(leagueVersionId, cancellationToken).ConfigureAwait(false);
}
