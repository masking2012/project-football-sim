using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;

namespace ProjectFootballSim.Leagues.Application.Catalog.Features.GetLeagueById;

public sealed class GetLeagueByIdQueryHandler(ILeaguesCatalog leagueCatalog)
{
    public ValueTask<LeagueDto?> HandleAsync(
        int leagueId,
        CancellationToken cancellationToken) =>
        leagueCatalog.GetByIdAsync(leagueId, cancellationToken);
}
