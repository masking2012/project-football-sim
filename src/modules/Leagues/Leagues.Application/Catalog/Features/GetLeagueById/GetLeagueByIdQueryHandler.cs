using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;

namespace ProjectFootballSim.Leagues.Application.Catalog.Features.GetLeagueById;

public sealed class GetLeagueByIdQueryHandler(ILeaguesCatalog leagueCatalog)
{
    public async ValueTask<LeagueDto?> HandleAsync(
        int leagueId,
        CancellationToken cancellationToken)
    {
        var leagues = await leagueCatalog.GetAllActiveAsync(cancellationToken).ConfigureAwait(false);
        return leagues.GetValueOrDefault(leagueId);
    }
}
